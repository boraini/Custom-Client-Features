private float MaxMiningDistance() {
	float mult = (this.AccessoryWithUse(Item.Use.BuildingExtension) == null) ? 1f : 2f;
	return Mathf.Lerp(3f, 5f, this.SkillLerp("mining")) * mult;
}
