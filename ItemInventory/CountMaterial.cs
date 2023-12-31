namespace T4.Plugins.DAV;

public class CountMaterial : BasePlugin, IGameUserInterfacePainter {
	public Feature TableSetting { get; private set; }

	public float SizeMultiplier { get; set; } = 0.8f;
	public IFont TextFont { get; set; } = Services.Render.GetFont(255, 255, 220, 220, fontFamily: "segoe ui", size: 9, shadowMode: FontShadowMode.Light);

	private ITexture bgIcon { get; set; } = Services.Render.GetTexture(SupportedTextureId.UIButtonLight_000);
	private List<MaterialData> matList { get; set; } = new List<MaterialData>();

	public void PaintGameUserInterface(GameUserInterfaceLayer layer) {
		if (layer != GameUserInterfaceLayer.OverPanels) return;
		if (!Services.UserInterface.InventoryAreaControl.Visible) return;

		var rect = Services.UserInterface.GetInventorySlotControl(0, 0, 2);
		var w = rect.Width * SizeMultiplier;
		var h = rect.Height * SizeMultiplier;
		var x = Services.UserInterface.InventoryAreaControl.Left;
		foreach (var mat in matList)
			mat.DrawCount(bgIcon, TextFont, x, rect.Top, w, h);
	}

	public class MaterialData {
		public string Name { get; private set; }
		public CurrencyType Currency { get; private set; }
		public ITexture Icon { get; private set; }

		private int xref { get; set; }
		private int yref { get; set; }

		// public MaterialData(ItemSnoId snoId, CurrencyType currency, uint iconId, int classType, int classRank) {
		public MaterialData(CurrencyType currency, ItemSnoId snoId, uint iconId, int classType, int classRank) {
			var material = Services.GameData.GetItemSno(snoId);
			Name = material.NameLocalized;
			Currency = currency;
			// Currency = material.CurrencyType;
			Icon = Services.Render.GetTexture(iconId);
			xref = classRank;
			yref = classType;
		}

		public void DrawCount(ITexture bgTexture, IFont font, float x, float y, float w, float h) {
			x -= w * xref;
			y -= h * yref;
			bgTexture.Draw(x, y, w, h);
			Icon.Draw(x, y, w, h);

			if (Services.Game.MyPlayer.Currencies.TryGetValue(Currency, out var count)) {
				var tl = font.GetTextLayout(count.ToString("#,##0"));
				tl.DrawText(x + w - tl.Width, y + h - tl.Height);
			}

			if (Services.Game.CursorInsideRect(x, y, w, h))
				Services.Hint.SetHint(Name);
		}
	}

/*
type	rank	CurrencyType	Name	ItemSnoId	SupportedTextureId
1	1	CommonHerb	Gallowvine	ItemSnoId.CraftingMaterial_Herb_Common	SupportedTextureId.InventoryMaterials_004
1	2	RareHerb	Angelbreath	ItemSnoId.CraftingMaterial_Herb_Rare	SupportedTextureId.InventoryMaterials_000
1	3	SuperRareHerb	FiendRose	ItemSnoId.CraftingMaterial_Herb_SuperRare	SupportedTextureId.InventoryMaterials_003
1	4	ScosglenHerb	HowlerMoss	ItemSnoId.CraftingMaterial_Herb_Scosglen	SupportedTextureId.InventoryMaterials_032
1	5	FracturedPeaksHerb	Biteberry	ItemSnoId.CraftingMaterial_Herb_FracturedPeaks	SupportedTextureId.InventoryMaterials_001
1	6	DrySteppesHerb	Reddamine	ItemSnoId.CraftingMaterial_Herb_Drysteppes	SupportedTextureId.InventoryMaterials_006
1	7	HawezarHerb	Blightshade	ItemSnoId.CraftingMaterial_Herb_Hawezar	SupportedTextureId.InventoryMaterials_002
1	8	KehjistanHerb	Lifesbane	ItemSnoId.CraftingMaterial_Herb_Kehjistan	SupportedTextureId.InventoryMaterials_005

2	1	CommonOre	IronChunk	ItemSnoId.CraftingMaterial_Ore_Common	SupportedTextureId.InventoryMaterials_018
2	2	RareOre	SilverOre	ItemSnoId.CraftingMaterial_Ore_Rare	SupportedTextureId.InventoryMaterials_023
2	3	RareScatteredPrism	ScatteredPrism	ItemSnoId.CraftingMaterial_Rare_ScatteredPrism	SupportedTextureId.InventoryMaterials_022

3	1	CommonLeather	Rawhide	ItemSnoId.CraftingMaterial_Leather_Common	SupportedTextureId.InventoryMaterials_020
3	2	RareLeather	SuperiorLeather	ItemSnoId.CraftingMaterial_Leather_Rare	SupportedTextureId.InventoryMaterials_026

4	1	DemonHeart	DemonsHeart	ItemSnoId.CraftingMaterial_MonsterDrop_Demon_Heart	SupportedTextureId.InventoryMaterials_014
4	2	HumanTongue	Paletongue	ItemSnoId.CraftingMaterial_MonsterDrop_Human_Tongue	SupportedTextureId.InventoryMaterials_019
4	3	UndeadDust	GraveDust	ItemSnoId.CraftingMaterial_MonsterDrop_Undead_Dust	SupportedTextureId.InventoryMaterials_016
4	4	WildlifeBones	CrushedBeastBones	ItemSnoId.CraftingMaterial_MonsterDrop_Wildlife_Bones	SupportedTextureId.InventoryMaterials_013

5	1	SacredLegendarySalvage	ForgottenSoul	ItemSnoId.CraftingMaterial_Salvage_Sacred_Legendary	SupportedTextureId.InventoryMaterials_015
5	2	RareSalvage	VeiledCrystal	ItemSnoId.CraftingMaterial_Salvage_Rare	SupportedTextureId.InventoryMaterials_031
5	3	LegendaryArmorSalvage	CoilingWard	ItemSnoId.CraftingMaterial_Salvage_Legendary_Armor	SupportedTextureId.InventoryMaterials_012
5	4	LegendaryJewelrySalvage	AbstruseSigil	ItemSnoId.CraftingMaterial_Salvage_Legendary_Jewelry	SupportedTextureId.InventoryMaterials_008
5	5	SigilSalvage	SigilPowder	ItemSnoId.CraftingMaterial_Salvage_Nightmare_Sigil_Powder	SupportedTextureId.InventoryQuestNature_001
5	6	LegendaryWeaponSalvage	BalefulFragment	ItemSnoId.CraftingMaterial_Salvage_Legendary_Weapon	SupportedTextureId.InventoryMaterials_007

*/

// initial setting
	public override string GetDescription() => Services.Translation.Translate(this, "display material summary at inventory");

	public override void Load() {
		base.Load();

		matList.Add(new MaterialData(CurrencyType.CommonHerb, ItemSnoId.CraftingMaterial_Herb_Common, SupportedTextureId.InventoryMaterials_004, 1, 1));
		matList.Add(new MaterialData(CurrencyType.RareHerb, ItemSnoId.CraftingMaterial_Herb_Rare, SupportedTextureId.InventoryMaterials_000, 1, 2));
		matList.Add(new MaterialData(CurrencyType.SuperRareHerb, ItemSnoId.CraftingMaterial_Herb_SuperRare, SupportedTextureId.InventoryMaterials_003, 1, 3));
		matList.Add(new MaterialData(CurrencyType.ScosglenHerb, ItemSnoId.CraftingMaterial_Herb_Scosglen, SupportedTextureId.InventoryMaterials_032, 1, 4));
		matList.Add(new MaterialData(CurrencyType.FracturedPeaksHerb, ItemSnoId.CraftingMaterial_Herb_FracturedPeaks, SupportedTextureId.InventoryMaterials_001, 1, 5));
		matList.Add(new MaterialData(CurrencyType.DrySteppesHerb, ItemSnoId.CraftingMaterial_Herb_Drysteppes, SupportedTextureId.InventoryMaterials_006, 1, 6));
		matList.Add(new MaterialData(CurrencyType.HawezarHerb, ItemSnoId.CraftingMaterial_Herb_Hawezar, SupportedTextureId.InventoryMaterials_002, 1, 7));
		matList.Add(new MaterialData(CurrencyType.KehjistanHerb, ItemSnoId.CraftingMaterial_Herb_Kehjistan, SupportedTextureId.InventoryMaterials_005, 1, 8));
		matList.Add(new MaterialData(CurrencyType.CommonOre, ItemSnoId.CraftingMaterial_Ore_Common, SupportedTextureId.InventoryMaterials_018, 2, 1));
		matList.Add(new MaterialData(CurrencyType.RareOre, ItemSnoId.CraftingMaterial_Ore_Rare, SupportedTextureId.InventoryMaterials_023, 2, 2));
		matList.Add(new MaterialData(CurrencyType.RareScatteredPrism, ItemSnoId.CraftingMaterial_Rare_ScatteredPrism, SupportedTextureId.InventoryMaterials_022, 2, 3));
		matList.Add(new MaterialData(CurrencyType.CommonLeather, ItemSnoId.CraftingMaterial_Leather_Common, SupportedTextureId.InventoryMaterials_020, 3, 1));
		matList.Add(new MaterialData(CurrencyType.RareLeather, ItemSnoId.CraftingMaterial_Leather_Rare, SupportedTextureId.InventoryMaterials_026, 3, 2));
		matList.Add(new MaterialData(CurrencyType.DemonHeart, ItemSnoId.CraftingMaterial_MonsterDrop_Demon_Heart, SupportedTextureId.InventoryMaterials_014, 4, 1));
		matList.Add(new MaterialData(CurrencyType.HumanTongue, ItemSnoId.CraftingMaterial_MonsterDrop_Human_Tongue, SupportedTextureId.InventoryMaterials_019, 4, 2));
		matList.Add(new MaterialData(CurrencyType.UndeadDust, ItemSnoId.CraftingMaterial_MonsterDrop_Undead_Dust, SupportedTextureId.InventoryMaterials_016, 4, 3));
		matList.Add(new MaterialData(CurrencyType.WildlifeBones, ItemSnoId.CraftingMaterial_MonsterDrop_Wildlife_Bones, SupportedTextureId.InventoryMaterials_013, 4, 4));
		matList.Add(new MaterialData(CurrencyType.SacredLegendarySalvage, ItemSnoId.CraftingMaterial_Salvage_Sacred_Legendary, SupportedTextureId.InventoryMaterials_015, 5, 1));
		matList.Add(new MaterialData(CurrencyType.RareSalvage, ItemSnoId.CraftingMaterial_Salvage_Rare, SupportedTextureId.InventoryMaterials_031, 5, 2));
		matList.Add(new MaterialData(CurrencyType.LegendaryArmorSalvage, ItemSnoId.CraftingMaterial_Salvage_Legendary_Armor, SupportedTextureId.InventoryMaterials_012, 5, 3));
		matList.Add(new MaterialData(CurrencyType.LegendaryJewelrySalvage, ItemSnoId.CraftingMaterial_Salvage_Legendary_Jewelry, SupportedTextureId.InventoryMaterials_008, 5, 4));
		matList.Add(new MaterialData(CurrencyType.SigilSalvage, ItemSnoId.CraftingMaterial_Salvage_Nightmare_Sigil_Powder, SupportedTextureId.InventoryQuestNature_001, 5, 5));
		matList.Add(new MaterialData(CurrencyType.LegendaryWeaponSalvage, ItemSnoId.CraftingMaterial_Salvage_Legendary_Weapon, SupportedTextureId.InventoryMaterials_007, 5, 6));

		TableSetting = new Feature() {
			Plugin = this,
			NameOf = nameof(TableSetting),
			DisplayName = () => Services.Translation.Translate(this, "item count on tabs"),
			Resources = new() {
				new FontFeatureResource() {
					NameOf = nameof(TextFont),
					DisplayText = () => Services.Translation.Translate(this, "font"),
					Font = TextFont,
				},
				new FloatFeatureResource() {
					NameOf = nameof(SizeMultiplier),
					DisplayText = () => Services.Translation.Translate(this, "size"),
					MinValue = 0.5f,
					MaxValue = 1f,
					Getter = () => SizeMultiplier,
					Setter = newValue => SizeMultiplier = MathF.Round(newValue, 1),
				},
			},
		};

		Services.Customization.RegisterFeature(TableSetting);
	}
}
