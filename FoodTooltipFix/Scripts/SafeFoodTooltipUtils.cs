/*
 * Copyright 2024 Peter Han
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using PeterHan.PLib.Core;
using System.Collections.Generic;
using UnityEngine;
using Klei.AI;

namespace PeterHan.FoodTooltip
{
    /// <summary>
    /// Utility functions used in Food Supply Tooltips.
    /// </summary>
    internal static class SafeFoodTooltipUtils
    {
        private const int CYCLES_FOR_SUMMARY = 5;

        internal static void AddCropDescriptors(Crop crop, IList<Descriptor> descriptors)
        {
            if (crop == null || descriptors == null)
                return;

            var db = Db.Get();
            if (crop.TryGetComponent(out Modifiers modifiers))
            {
                var cropVal = crop.cropVal;
                float preModifiedAttributeValue = modifiers.GetPreModifiedAttributeValue(
                    db.PlantAttributes.YieldAmount);
                var maturity = Db.Get().Amounts.Maturity.Lookup(crop);
                if (maturity != null)
                {
                    CreateDescriptorsSafe(TagManager.Create(cropVal.cropId), descriptors,
                        maturity.GetDelta() * preModifiedAttributeValue * Constants.SECONDS_PER_CYCLE / maturity.GetMax(),
                        FoodDescriptorTexts.PLANTS);
                }
            }
        }

        internal static void AddCritterDescriptors(GameObject critter, IList<Descriptor> descriptors)
        {
            if (critter == null || descriptors == null)
                return;

            IDictionary<string, float> drops;
            if (critter.TryGetComponent(out Butcherable butcher) && (drops = butcher.drops) != null && drops.Count > 0)
            {
                GetEggsPerCycle(critter, out float replacement, out float noReplacement);

                foreach (var pair in drops)
                {
                    if (pair.Key != null)
                        CreateDescriptorsSafe(TagManager.Create(pair.Key), descriptors, pair.Value * noReplacement, FoodDescriptorTexts.CRITTERS);
                }

                var fertDef = critter.GetDef<FertilityMonitor.Def>();
                if (fertDef?.eggPrefab != null)
                    CreateDescriptorsSafe(fertDef.eggPrefab, descriptors, replacement, FoodDescriptorTexts.CRITTERS);
            }
        }

        private static void CreateDescriptorsSafe(Tag drop, ICollection<Descriptor> descriptors, float dropRate, FoodDescriptorText text)
        {
            if (drop == null || descriptors == null || text == null)
                return;

            string dropName = drop.ProperName();
            var foodCache = FoodRecipeCache.Instance;
            if (foodCache == null)
                return;

            foreach (var food in foodCache.Lookup(drop))
            {
                if (food.Result == null)
                    continue;

                string foodName = food.Result.ProperName();
                if (dropRate > 0.0f)
                {
                    string perCycle = GameUtil.AddTimeSliceText(GameUtil.GetFormattedCalories(food.Calories * dropRate * food.Quantity),
                        GameUtil.TimeSlice.PerCycle);

                    descriptors.Add(new Descriptor(((string)text.PerCycle).F(foodName, perCycle, dropName),
                                                   ((string)text.PerCycleTooltip).F(foodName, perCycle, dropName)));
                }
                else
                {
                    descriptors.Add(new Descriptor(((string)text.Stifled).F(foodName),
                                                   (string)text.StifledTooltip));
                }
            }
        }

        private static string FormatDeltaTooltip(string text, float produced, float consumed)
        {
            return text.F(GameUtil.GetFormattedCalories(produced), GameUtil.GetFormattedCalories(consumed));
        }

        private static void GetCalorieDeltas(ReportManager.DailyReport report, out float produced, out float consumed)
        {
            ReportManager.ReportEntry entry;
            if ((entry = report?.GetEntry(ReportManager.ReportType.CaloriesCreated)) != null)
            {
                produced = entry.accPositive;
                consumed = -entry.accNegative;
            }
            else
            {
                produced = 0.0f;
                consumed = 0.0f;
            }
        }

        private static void GetEggsPerCycle(GameObject obj, out float replacement, out float noReplacement)
        {
            var amounts = Db.Get().Amounts;
            var fertility = amounts.Fertility.Lookup(obj);
            var age = amounts.Age.Lookup(obj);
            float delta;
            if (fertility != null && age != null && (delta = fertility.GetDelta()) > 0.0f)
            {
                float maxAge = age.GetMax(), totalEggs = maxAge * delta * Constants.SECONDS_PER_CYCLE / fertility.GetMax();
                noReplacement = Mathf.Floor(totalEggs) / maxAge;
                replacement = Mathf.Floor(Mathf.Max(0.0f, totalEggs - 1.0f)) / maxAge;
            }
            else
            {
                replacement = 0.0f;
                noReplacement = 0.0f;
            }
        }

        internal static void ShowFoodUseStats(ToolTip tooltip, TextStyleSetting style)
        {
            var reports = ReportManager.Instance;
            if (tooltip == null || reports == null)
                return;

            GetCalorieDeltas(reports.TodaysReport, out float produced, out float consumed);
            tooltip.AddMultiStringTooltip(FormatDeltaTooltip(FoodTooltipStrings.FOOD_RATE_CURRENT, produced, consumed), style);

            GetCalorieDeltas(reports.YesterdaysReport, out produced, out consumed);
            tooltip.AddMultiStringTooltip(FormatDeltaTooltip(FoodTooltipStrings.FOOD_RATE_LAST1, produced, consumed), style);

            int days = 0, cycle = GameUtil.GetCurrentCycle();
            float totalProduced = 0.0f, totalConsumed = 0.0f;
            foreach (var report in reports.reports)
            {
                if (report.day >= cycle - CYCLES_FOR_SUMMARY)
                {
                    GetCalorieDeltas(report, out produced, out consumed);
                    totalProduced += produced;
                    totalConsumed += consumed;
                    days++;
                }
            }
            if (days == 0)
                days = 1;
            tooltip.AddMultiStringTooltip(FormatDeltaTooltip(FoodTooltipStrings.FOOD_RATE_LAST5, totalProduced / days, totalConsumed / days), style);
        }
    }
}
