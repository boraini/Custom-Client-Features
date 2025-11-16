private void OnAccessoriesChanged(List<Item> accessories) {
    List<Item> effectableItems = ReplaceableSingleton<Player>.main.inventory.AllEffectableItems();
    Dictionary<string, int> stacking = new Dictionary<string, int>();
    foreach (Item item in effectableItems) {
        if (item.IsUsableType(Item.Use.SkillBonus)) {
            foreach (KeyValuePair<string, int> keyValuePair in item.skillBonuses) {
                if (ReplaceableSingleton<Player>.main.Skill(keyValuePair.Key) > 0) {
                    if (stacking.ContainsKey(keyValuePair.Key)) {
                        stacking[keyValuePair.Key] = Math.Max(stacking[keyValuePair.Key], keyValuePair.Value);
                    } else {
                        stacking[keyValuePair.Key] = keyValuePair.Value;
                    }
                }
            }
        }
    }
    List<string> lines = new List<string>();
    foreach (KeyValuePair<string, int> keyValuePair2 in stacking) {
        lines.Add(string.Concat(new object[] { "+", keyValuePair2.Value, " ", keyValuePair2.Key }));
    }
    foreach (Item item2 in effectableItems) {
        if (item2.shortHint != null) {
            lines.Add(item2.shortHint);
        }
    }
    lines.Sort();
    this.bonusesText.text = string.Join("\n", lines.ToArray());
}
