using KSerialization;
using UnityEngine;

namespace AutoDrop
{
    // 自动化标签组件
    public class AutomatableLable : Automatable
    {
        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
        }

        [SerializeField]
        public bool showInUI = false;
    }

    // 自动丢弃组件
    public class AutoDropComponent : KMonoBehaviour
    {
        private bool isActive = true;
        private Storage storage;
        private DropAllWorkable dropper;
        private Bottler bottler;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            this.storage = base.GetComponent<Storage>();
            this.dropper = base.GetComponent<DropAllWorkable>();
            this.bottler = base.GetComponent<Bottler>();
            base.Subscribe(-1697596308, new System.Action<object>(this.OnStorageChanged));
            base.Subscribe(-1697596308, new System.Action<object>(this.OnCapacityChanged));
        }

        private void ForceStorageCheck()
        {
            bool flag = !this.isActive || !this.IsComponentsValid() || this.storage.MassStored() < this.bottler.userMaxCapacity;
            if (!flag)
            {
                this.dropper.CompleteWork(null);
            }
        }

        private void OnStorageChanged(object data)
        {
            this.CheckAndDrop();
        }

        private void OnCapacityChanged(object data)
        {
            this.CheckAndDrop();
        }

        private void CheckAndDrop()
        {
            bool flag = !this.isActive || !this.IsComponentsValid() || this.storage.MassStored() < this.bottler.userMaxCapacity;
            if (!flag)
            {
                this.dropper.CompleteWork(null);
            }
        }

        private bool IsComponentsValid()
        {
            return true;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.ForceStorageCheck();
        }

        public void Disable()
        {
            this.isActive = false;
        }
    }

    // 切换按钮组件
    public class AutoDropToggle : KMonoBehaviour
    {
        [Serialize]
        public bool automationOnly = false;

        private Automatable automatable;

        [MyCmpAdd]
        private static readonly EventSystem.IntraObjectHandler<AutoDropToggle> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<AutoDropToggle>(delegate (AutoDropToggle component, object data)
        {
            component.OnCopySettings(data);
        });

        private static readonly EventSystem.IntraObjectHandler<AutoDropToggle> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<AutoDropToggle>(delegate (AutoDropToggle component, object data)
        {
            component.OnRefreshUserMenu(data);
        });

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            base.Subscribe<AutoDropToggle>(-905833192, AutoDropToggle.OnCopySettingsDelegate);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            base.Subscribe<AutoDropToggle>(493375141, AutoDropToggle.OnRefreshUserMenuDelegate);
            this.automatable = base.gameObject.GetComponent<Automatable>();
            bool flag = this.automatable == null;
            if (flag)
            {
                this.automatable = base.gameObject.AddComponent<AutomatableLable>();
            }
            this.ApplyState();
        }

        private void OnCopySettings(object data)
        {
            AutoDropToggle component = ((GameObject)data).GetComponent<AutoDropToggle>();
            bool flag = component != null;
            if (flag)
            {
                this.automationOnly = component.automationOnly;
                this.ApplyState();
            }
        }

        private void ToggleState()
        {
            this.automationOnly = !this.automationOnly;
            this.ApplyState();
        }

        private void ApplyState()
        {
            this.automatable.SetAutomationOnly(this.automationOnly);
            AutoDropComponent component = base.gameObject.GetComponent<AutoDropComponent>();
            bool flag = this.automationOnly;
            if (flag)
            {
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Disable();
                    UnityEngine.Object.Destroy(component);
                }
            }
            else
            {
                bool flag3 = component == null;
                if (flag3)
                {
                    base.gameObject.AddComponent<AutoDropComponent>();
                }
            }
        }

        private void OnRefreshUserMenu(object data)
        {
            // 直接使用本地化字符串
            string text = this.automationOnly ?
                STRINGS.UI.USERMENUACTIONS.AUTODROP_ENABLE.NAME :
                STRINGS.UI.USERMENUACTIONS.AUTODROP_DISABLE.NAME;

            string tooltipText = this.automationOnly ?
                STRINGS.UI.USERMENUACTIONS.AUTODROP_ENABLE.TOOLTIP :
                STRINGS.UI.USERMENUACTIONS.AUTODROP_DISABLE.TOOLTIP;

            Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_empty_contents", text, new System.Action(this.ToggleState), global::Action.NumActions, null, null, null, tooltipText, true), 1f);
        }
    }
}