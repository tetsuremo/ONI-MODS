using PeterHan.PLib.Options;
using System;

namespace WaterGateKai
{
    /// <summary>
    /// 液体类型枚举 - 按照wiki页面分类准确列出
    /// </summary>
    public enum LiquidType
    {
        // ==================== Water Based ====================
        [Option("STRINGS.ELEMENTS.BRINE.NAME", "")]
        Brine,

        [Option("STRINGS.ELEMENTS.DIRTYWATER.NAME", "")]
        DirtyWater,

        [Option("STRINGS.ELEMENTS.SALTWATER.NAME", "")]
        SaltWater,

        [Option("STRINGS.ELEMENTS.WATER.NAME", "")]
        Water,

        // ==================== Molten Metal ====================
        [Option("STRINGS.ELEMENTS.MOLTENURANIUM.NAME", "")]
        MoltenUranium,

        [Option("STRINGS.ELEMENTS.MERCURY.NAME", "")]
        Mercury,

        [Option("STRINGS.ELEMENTS.MOLTENALUMINUM.NAME", "")]
        MoltenAluminum,

        [Option("STRINGS.ELEMENTS.MOLTENCOBALT.NAME", "")]
        MoltenCobalt,

        [Option("STRINGS.ELEMENTS.MOLTENCOPPER.NAME", "")]
        MoltenCopper,

        [Option("STRINGS.ELEMENTS.MOLTENGOLD.NAME", "")]
        MoltenGold,

        [Option("STRINGS.ELEMENTS.MOLTENIRIDIUM.NAME", "")]
        MoltenIridium,

        [Option("STRINGS.ELEMENTS.MOLTENIRON.NAME", "")]
        MoltenIron,

        [Option("STRINGS.ELEMENTS.MOLTENLEAD.NAME", "")]
        MoltenLead,

        [Option("STRINGS.ELEMENTS.MOLTENNICKEL.NAME", "")]
        MoltenNickel,

        [Option("STRINGS.ELEMENTS.MOLTENNIOBIUM.NAME", "")]
        MoltenNiobium,

        [Option("STRINGS.ELEMENTS.MOLTENSTEEL.NAME", "")]
        MoltenSteel,

        [Option("STRINGS.ELEMENTS.MOLTENTUNGSTEN.NAME", "")]
        MoltenTungsten,

        // ==================== Oil Based ====================
        [Option("STRINGS.ELEMENTS.CRUDEOIL.NAME", "")]
        CrudeOil,

        [Option("STRINGS.ELEMENTS.LIQUIDGUNK.NAME", "")]
        LiquidGunk,

        [Option("STRINGS.ELEMENTS.LIQUIDMETHANE.NAME", "")]
        LiquidMethane,

        [Option("STRINGS.ELEMENTS.NAPHTHA.NAME", "")]
        Naphtha,

        [Option("STRINGS.ELEMENTS.PETROLEUM.NAME", "")]
        Petroleum,

        [Option("STRINGS.ELEMENTS.PHYTOOIL.NAME", "")]
        PhytoOil,

        [Option("STRINGS.ELEMENTS.VISCOGEL.NAME", "")]
        ViscoGel,

        // ==================== Liquid ====================
        [Option("STRINGS.ELEMENTS.REFINEDLIPID.NAME", "")]
        RefinedLipid,

        [Option("STRINGS.ELEMENTS.MILK.NAME", "")]
        Milk,

        [Option("STRINGS.ELEMENTS.ETHANOL.NAME", "")]
        Ethanol,

        [Option("STRINGS.ELEMENTS.MOLTENCARBON.NAME", "")]
        MoltenCarbon,

        [Option("STRINGS.ELEMENTS.LIQUIDCARBONDIOXIDE.NAME", "")]
        LiquidCarbonDioxide,

        [Option("STRINGS.ELEMENTS.CHLORINE.NAME", "")]
        LiquidChlorine,

        [Option("STRINGS.ELEMENTS.LIQUIDHYDROGEN.NAME", "")]
        LiquidHydrogen,

        [Option("STRINGS.ELEMENTS.NUCLEARWASTE.NAME", "")]
        NuclearWaste,

        [Option("STRINGS.ELEMENTS.LIQUIDOXYGEN.NAME", "")]
        LiquidOxygen,

        [Option("STRINGS.ELEMENTS.LIQUIDPHOSPHORUS.NAME", "")]
        LiquidPhosphorus,

        [Option("STRINGS.ELEMENTS.MOLTENSUCROSE.NAME", "")]
        MoltenSucrose,

        [Option("STRINGS.ELEMENTS.LIQUIDSULFUR.NAME", "")]
        LiquidSulfur,

        [Option("STRINGS.ELEMENTS.MAGMA.NAME", "")]
        Magma,

        [Option("STRINGS.ELEMENTS.MOLTENGLASS.NAME", "")]
        MoltenGlass,

        [Option("STRINGS.ELEMENTS.MOLTENSALT.NAME", "")]
        MoltenSalt,

        [Option("STRINGS.ELEMENTS.SUGARWATER.NAME", "")]
        SugarWater,

        [Option("STRINGS.ELEMENTS.NATURALRESIN.NAME", "")]
        NaturalResin,

        [Option("STRINGS.ELEMENTS.RESIN.NAME", "")]
        Resin,

        [Option("STRINGS.ELEMENTS.SUPERCOOLANT.NAME", "")]
        SuperCoolant
    }
}