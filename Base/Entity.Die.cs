public virtual void Die(Dictionary<string, object> details = null) {
    if (details != null) {
        if (details.GetString("!") == "v") {
            this.DieViolently();
        }
        int killerId = details.GetInt("<", 0);
        if (killerId != 0) {
            Entity killer = ReplaceableSingleton<Ecosystem>.main.GetEntity(killerId);
            if (killer != null) {
                if (this.isPlayer) {
                    Notification.Create(killer.name + " killed you!", 1);
                } else {
                    Notification.Create(killer.name + " killed " + base.name + ".", 1);
                }
            }
        }
    }
    this._alive = false;
}
