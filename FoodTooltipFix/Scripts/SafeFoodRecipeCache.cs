using System;
using System.Collections.Generic;

namespace PeterHan.FoodTooltip
{
    public sealed class FoodRecipeCache : IDisposable
    {
        public static FoodRecipeCache Instance { get; private set; }
        private static readonly Tag MILK_TAG = SimHashes.Milk.CreateTag();
        private readonly IDictionary<Tag, IList<FoodResult>> cache;

        private FoodRecipeCache()
        {
            cache = new Dictionary<Tag, IList<FoodResult>>(64);
        }

        public static void CreateInstance()
        {
            Instance = new FoodRecipeCache();
        }

        public static void DestroyInstance()
        {
            Instance?.Dispose();
            Instance = null;
        }

        private static void SearchForRecipe(Tag item, ICollection<FoodResult> found, ISet<Tag> seen, float quantity)
        {
            var prefab = Assets.GetPrefab(item);
            if (prefab == null || quantity <= 0 || !seen.Add(item) || item == MILK_TAG) return;

            if (prefab.TryGetComponent(out Edible edible) && edible.FoodInfo.CaloriesPerUnit > 0f)
                found.Add(new FoodResult(edible.FoodInfo.CaloriesPerUnit, quantity, item));

            foreach (var recipe in RecipeManager.Get().recipes)
            {
                float amount = 0f;
                foreach (var ing in recipe.Ingredients)
                    if (ing.tag == item) { amount = ing.amount; break; }
                if (amount > 0f)
                    SearchForRecipe(recipe.Result, found, seen, recipe.OutputUnits * quantity / amount);
            }

            foreach (var recipe in ComplexRecipeManager.Get().recipes)
            {
                if (recipe.fabricators.Contains(FoodDehydratorConfig.ID)) continue;
                float amount = 0f;
                foreach (var ing in recipe.ingredients)
                    if (ing.material == item) { amount = ing.amount; break; }
                if (amount > 0f)
                    foreach (var result in recipe.results)
                        SearchForRecipe(result.material, found, seen, result.amount * quantity / amount);
            }
        }

        public FoodResult[] Lookup(Tag tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            if (!cache.TryGetValue(tag, out var items))
            {
                var seen = HashSetPool<Tag, FoodRecipeCache>.Allocate();
                try
                {
                    items = new List<FoodResult>();
                    SearchForRecipe(tag, items, seen, 1.0f);
                    cache.Add(tag, items);
                }
                finally
                {
                    seen.Recycle();
                }
            }

            var result = new FoodResult[items.Count];
            items.CopyTo(result, 0);
            return result;
        }

        public void Dispose()
        {
            cache.Clear();
        }

        public readonly struct FoodResult
        {
            public float Calories { get; }
            public float Quantity { get; }
            public Tag Result { get; }
            public FoodResult(float kcal, float quantity, Tag result)
            {
                Calories = kcal;
                Quantity = quantity;
                Result = result;
            }
            public override string ToString() => $"FoodResult[{Result},qty={Quantity:F2},kcal={Calories:F0}]";
        }
    }
}
