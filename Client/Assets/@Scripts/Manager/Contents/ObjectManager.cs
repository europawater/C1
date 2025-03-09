using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class ObjectManager
{
	public Map Map { get; private set; }
	public Hero Hero { get; private set; }
	public HashSet<BaseMonsterObject> Monsters { get; private set; } = new HashSet<BaseMonsterObject>();
	public List<BaseMonsterObject> LivingMonsterList
	{
		get { return Monsters.Where(monster => monster.IsAlive).ToList(); }
	}

	public List<SkillEffect> SkillEffects { get; private set; } = new List<SkillEffect>();
	public List<SkillCube> SkillCubes { get; private set; } = new List<SkillCube>();

	public List<Projectile> Projectiles { get; private set; } = new List<Projectile>();

	#region Spawn

	private const string HERO_PREFAB_NAME = "Hero";
	private const string NORMAL_MONSTER_PREFAB_NAME = "NormalMonster";
	private const string BOSS_MONSTER_PREFAB_NAME = "BossMonster";

	private const string SKILL_CUBE_PREFAB_NAME = "SkillCube";

	public Map SpawnMap(Vector3 position, string prefabName)
	{
		Map map = Managers.Resource.Instantiate(prefabName).GetComponent<Map>();
		if (map != null)
		{
			Map = map;
			map.transform.position = position;
			return map;
		}

		return null;
	}

	public T SpawnAIObject<T>(Transform parent, int templateID, int difficultyLevel = 0) where T : BaseAIObject
	{
		T aiObject = null;

		if (typeof(T) == typeof(Hero))
		{
			aiObject = SpawnHero(parent, templateID) as T;
			return aiObject;
		}
		else if (typeof(T) == typeof(NormalMonster))
		{
			aiObject = SpawnNormalMonster(parent, templateID, difficultyLevel) as T;
			return aiObject;
		}
		else if (typeof(T) == typeof(BossMonster))
		{
			aiObject = SpawnBossMonster(parent, templateID, difficultyLevel) as T;
			return aiObject;
		}

		return null;
	}

	private Hero SpawnHero(Transform parent, int templateID)
	{
		Hero hero = Managers.Resource.Instantiate(HERO_PREFAB_NAME, parent).GetComponent<Hero>();
		if (hero != null)
		{
			Hero = hero;
			hero.SetInfo(templateID);
			return hero;
		}

		return null;
	}

	private NormalMonster SpawnNormalMonster(Transform parent, int templateID, int difficultyLevel)
	{
		NormalMonster normalMonster = Managers.Resource.Instantiate(NORMAL_MONSTER_PREFAB_NAME, parent).GetComponent<NormalMonster>();
		if (normalMonster != null)
		{
			normalMonster.SetInfo(templateID, difficultyLevel);
			Monsters.Add(normalMonster);
			return normalMonster;
		}

		return null;
	}

	private BossMonster SpawnBossMonster(Transform parent, int templateID, int difficultyLevel)
	{
		BossMonster bossMonster = Managers.Resource.Instantiate(BOSS_MONSTER_PREFAB_NAME, parent).GetComponent<BossMonster>();
		if (bossMonster != null)
		{
			bossMonster.SetInfo(templateID, difficultyLevel);
			Monsters.Add(bossMonster);
			return bossMonster;
		}

		return null;
	}

	public SkillEffect SpawnSkillEffect(Vector3 position, string prefabName, float lifeTime)
	{
		SkillEffect skillEffect = Managers.Resource.Instantiate(prefabName).GetOrAddComponent<SkillEffect>();
		if (skillEffect != null)
		{
			skillEffect.SetInfo(lifeTime);
			skillEffect.transform.position = position;
			SkillEffects.Add(skillEffect);
			return skillEffect;
		}

		return null;
	}

	public SkillCube SpawnSkillCube(Vector3 position)
	{ 
		SkillCube skillCube = Managers.Resource.Instantiate(SKILL_CUBE_PREFAB_NAME).GetOrAddComponent<SkillCube>();
		if(skillCube != null)
		{
			skillCube.transform.position = position;
			SkillCubes.Add(skillCube);
			return skillCube;
		}

		return null;
	}

	public Projectile SpawnProjectile(string prefabName, Vector2 startPosition, Vector2 endPosition, float lifeTime)
	{
		Projectile projectile = Managers.Resource.Instantiate(prefabName).GetOrAddComponent<Projectile>();
		if (projectile != null)
		{
			projectile.SetInfo(startPosition, endPosition, lifeTime);
			projectile.transform.position = startPosition;
			Projectiles.Add(projectile);
			return projectile;
		}
		return null;
	}

	#endregion

	#region Remove

	public void RemoveMap()
	{ 
		if(Map != null)
		{
			Managers.Resource.Destroy(Map.gameObject);
			Map = null;
		}
	}

	public void RemoveHero()
	{
		if (Hero != null)
		{
			Managers.Resource.Destroy(Hero.gameObject);
			Hero = null;
		}
	}

	public void RemoveMonster(BaseMonsterObject monster)
	{
		if (Monsters.Contains(monster))
		{
			Managers.Resource.Destroy(monster.gameObject);
			Monsters.Remove(monster);
		}
	}

	public void RemoveAllMonsters()
	{
		foreach (BaseMonsterObject monster in Monsters)
		{
			Managers.Resource.Destroy(monster.gameObject);
		}

		Monsters.Clear();
	}

	public void RemoveSkillEffect(SkillEffect skillEffect)
	{
		if (SkillEffects.Contains(skillEffect))
		{
			Managers.Resource.Destroy(skillEffect.gameObject);
			SkillEffects.Remove(skillEffect);
		}
	}

	public void RemoveAllSkillEffect()
	{
		foreach (SkillEffect skillEffect in SkillEffects)
		{
			Managers.Resource.Destroy(skillEffect.gameObject);
		}

		SkillEffects.Clear();
	}

	public void RemoveSkillCube(SkillCube skillCube)
	{
		if (SkillCubes.Contains(skillCube))
		{
			Managers.Resource.Destroy(skillCube.gameObject);
			SkillCubes.Remove(skillCube);
		}
	}

	public void RemoveAllSkillCube()
	{
		foreach (SkillCube skillCube in SkillCubes)
		{
			Managers.Resource.Destroy(skillCube.gameObject);
		}

		SkillCubes.Clear();
	}

	public void RemoveProjectile(Projectile projectile)
	{
		if (Projectiles.Contains(projectile))
		{
			Managers.Resource.Destroy(projectile.gameObject);
			Projectiles.Remove(projectile);
		}
	}

	#endregion

	#region Util

	public void SetAllAIObjectState(EAIObjectState state)
	{
		if (Hero != null)
		{
			Hero.AIObjectState = state;
		}

		foreach (BaseMonsterObject monster in Monsters)
		{
			monster.AIObjectState = state;
		}
	}

	#endregion
}
