namespace T4.Plugins.DAV;

public class HerbOre : BasePlugin, IRenderEnabler, IGameWorldPainter {
	public Feature IconSetting { get; private set; }

	public float SizeMap { get; set; } = 5f;
	public float SizeScreen { get; set; } = 10f;

	private IEnumerable<IGizmoActor> matList { get; set; }
	private Dictionary<ActorSnoId, bool> actorList { get; set; }
	private ITexture iconMap { get; set; } = Services.Render.GetTexture(SupportedTextureId.UIMinimapIcons_109);
	private ITexture iconOre { get; set; } = Services.Render.GetTexture(SupportedTextureId.UIChallengesCrafting01_012);
	private ITexture iconHerb { get; set; } = Services.Render.GetTexture(SupportedTextureId.UIChallengesCrafting01_027);

	public bool BeforeRender() {
		matList = Services.Game.GizmoActors.Where(x => actorList.TryGetValue(x.ActorSno.SnoId, out var enabled) && enabled);

		return true;
	}

	public void PaintGameWorld(GameWorldLayer layer) {
		if (matList.Count() == 0) return;

		if (layer == GameWorldLayer.Map) {
			if (SizeMap <= 0) return;

			var sizeMap = Services.Game.WindowHeight / 108f * SizeMap;
			var sizeMap2 = sizeMap / 2;
			foreach (var actor in matList) {
				if (actor.Coordinate.IsOnMap)
					iconMap.Draw(actor.Coordinate.MapX - sizeMap2, actor.Coordinate.MapY - sizeMap2, sizeMap, sizeMap, false);
			}
		}
		else if (SizeScreen > 0) {
			var sizeScreen = Services.Game.WindowHeight / 108f * SizeScreen;
			var sizeScreen2 = sizeScreen / 2;
			foreach (var actor in matList) {
				if (actor.Coordinate.IsOnScreen)
					DrawIcon(actor.ActorSno.SnoId, actor.Coordinate.ScreenX - sizeScreen2, actor.Coordinate.ScreenY - sizeScreen2, sizeScreen, false);
			}
		}
	}

	private void DrawIcon(ActorSnoId actorId, float x, float y, float size, bool sharp) {
		switch (actorId) {
			case ActorSnoId.HarvestNode_Ore_Global_Common :
			case ActorSnoId.HarvestNode_Ore_Global_Common_PROLOGUE :
			case ActorSnoId.HarvestNode_Ore_Global_Rare :
			case ActorSnoId.USZ_HarvestNode_Ore_UberSubzone_001_Dyn :
				iconOre.Draw(x, y, size, size, sharp);
				break;
			default :
				iconHerb.Draw(x, y, size, size, sharp);
				break;
		}
	}

// initial setting
	public override string GetDescription() => Services.Translation.Translate(this, "show the nearby herbs/ores");

	public override void Load() {
		base.Load();

		var actorSnoIds = new ActorSnoId[] {
			ActorSnoId.HarvestNode_Herb_Common_Gallowvine,
			ActorSnoId.HarvestNode_Herb_Common_Gallowvine_PROLOGUE,
			ActorSnoId.HarvestNode_Herb_Frac_Biteberry,
			ActorSnoId.HarvestNode_Herb_Frac_Biteberry_PROLOGUE,
			ActorSnoId.HarvestNode_Herb_Hawe_Blightshade,
			ActorSnoId.HarvestNode_Herb_Kehj_Lifesbane,
			ActorSnoId.HarvestNode_Herb_Rare_Angelbreath,
			ActorSnoId.HarvestNode_Herb_Rare_FiendRose,
			ActorSnoId.HarvestNode_Herb_Scos_HowlerMoss,
			ActorSnoId.HarvestNode_Herb_Step_Reddamine,

			ActorSnoId.HarvestNode_Ore_Global_Common,
			ActorSnoId.HarvestNode_Ore_Global_Common_PROLOGUE,
			ActorSnoId.HarvestNode_Ore_Global_Rare,
			ActorSnoId.USZ_HarvestNode_Ore_UberSubzone_001_Dyn
		};

		actorList = actorSnoIds.ToDictionary(x => x, x => true);

		IconSetting = new Feature() {
			Plugin = this,
			NameOf = nameof(IconSetting),
			DisplayName = () => Services.Translation.Translate(this, "icon on screen / map"),
			Resources = new() {
				new FloatFeatureResource() {
					NameOf = nameof(SizeScreen),
					DisplayText = () => Services.Translation.Translate(this, "icon size - screen"),
					MinValue = 0.0f,
					MaxValue = 20.0f,
					Getter = () => SizeScreen,
					Setter = newValue => SizeScreen = MathF.Round(newValue, 1),
				},
				new FloatFeatureResource() {
					NameOf = nameof(SizeMap),
					DisplayText = () => Services.Translation.Translate(this, "icon size - map"),
					MinValue = 0.0f,
					MaxValue = 10.0f,
					Getter = () => SizeMap,
					Setter = newValue => SizeMap = MathF.Round(newValue, 1),
				},
			},
		};

		for (var i = 0; i < actorSnoIds.Length; i++) {
			var actorSnoId = actorSnoIds[i];
			IconSetting.Resources.Add(new BooleanFeatureResource() {
				NameOf = actorSnoId.ToString(),
				DisplayText = () => Services.GameData.GetActorSno(actorSnoId).NameLocalized,
				Getter = () => actorList[actorSnoId], // .TryGetValue(actorSnoId, out var enabled) && enabled,
				Setter = newValue => actorList[actorSnoId] = newValue,
			});
		}

		Services.Customization.RegisterFeature(IconSetting);
	}
}
