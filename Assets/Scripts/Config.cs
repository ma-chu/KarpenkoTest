[System.Serializable]
public struct ConfigurableParams
{
	public float notWalkablePart;
	public int width;
	public int height;
	public int numberOfUnits;

	public float closeCombatUnitDamageBaseMin;
	public float closeCombatUnitDamageBaseMax;
	public float closeCombatUnitAttackRange;
	public float closeCombatUnitAttackTime;
	public float closeCombatUnitMovementSpeed;
	public float closeCombatUnitStartingHealth;

	public float distantCombatUnitDamageBaseMin;
	public float distantCombatUnitDamageBaseMax;
	public float distantCombatUnitAttackRange;
	public float distantCombatUnitAttackTime;
	public float distantCombatUnitMovementSpeed;
	public float distantCombatUnitStartingHealth;
	public float bulletVelocity;
}