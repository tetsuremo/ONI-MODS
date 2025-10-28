using System.Collections.Generic;
using Klei.AI;
using PeterHan.PLib.Core;
using UnityEngine;

namespace PeterHan.FoodTooltip
{
    [SkipSaveFileSerialization]
    internal sealed class InfoScreenRefresher : KMonoBehaviour
    {
        private static readonly ICollection<string> EFFECTS = new HashSet<string>() { "Happy", "Neutral", "Glum", "Miserable", "FarmTinker" };

#pragma warning disable CS0649
        [MyCmpGet]
        private SimpleInfoScreen infoScreen;
#pragma warning restore CS0649

        private void EffectRefresh(object data)
        {
            if (data is Effect effect && EFFECTS.Contains(effect.Id))
            {
                var di = DetailsScreen.Instance;
                if (infoScreen != null && di != null && di.isActiveAndEnabled && di.target != null)
                    infoScreen.RefreshInfoScreen(true);
            }
        }

        internal void OnDeselectTarget(GameObject target)
        {
            if (target != null)
            {
                Unsubscribe(target, (int)GameHashes.EffectAdded, EffectRefresh);
                Unsubscribe(target, (int)GameHashes.EffectRemoved, EffectRefresh);
                Unsubscribe(target, (int)GameHashes.Wilt, RefreshInfoPanel);
                Unsubscribe(target, (int)GameHashes.WiltRecover, RefreshInfoPanel);
                Unsubscribe(target, (int)GameHashes.CropSleep, RefreshInfoPanel);
                Unsubscribe(target, (int)GameHashes.CropWakeUp, RefreshInfoPanel);
            }
        }

        internal void OnSelectTarget(GameObject target)
        {
            if (target != null)
            {
                Subscribe(target, (int)GameHashes.EffectAdded, EffectRefresh);
                Subscribe(target, (int)GameHashes.EffectRemoved, EffectRefresh);
                Subscribe(target, (int)GameHashes.Wilt, RefreshInfoPanel);
                Subscribe(target, (int)GameHashes.WiltRecover, RefreshInfoPanel);
                Subscribe(target, (int)GameHashes.CropSleep, RefreshInfoPanel);
                Subscribe(target, (int)GameHashes.CropWakeUp, RefreshInfoPanel);
            }
        }

        private void RefreshInfoPanel(object _)
        {
            var di = DetailsScreen.Instance;
            if (infoScreen != null && di != null && di.isActiveAndEnabled && di.target != null)
                infoScreen.RefreshInfoScreen(true);
        }
    }
}
