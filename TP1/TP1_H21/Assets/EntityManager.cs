using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// À rendre plus meilleur!
/// 
/// (Polymorphisme pour les listes? Listes de listes?)
/// </summary>
public class EntityManager
{
    private List<CircleComponent> circleComponents;
    public List<CircleComponent> CircleComponents => circleComponents;

    private List<PositionComponent> positionComponents;
    public List<PositionComponent> PositionComponents => positionComponents;

    private List<SpeedComponent> speedComponents;
    public List<SpeedComponent> SpeedComponents => speedComponents;

    #region Singleton
    private static EntityManager _instance;
    public static EntityManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EntityManager();
            }
            return _instance;
        }
    }
    #endregion

    public EntityManager()
    {
        circleComponents = new List<CircleComponent>();
        positionComponents = new List<PositionComponent>();
        speedComponents = new List<SpeedComponent>();
    }
    public void AddCircleComponent(CircleComponent newCircle)
    {
        circleComponents.Add(newCircle);
    }
    public void AddPositionComponent(PositionComponent newPosition)
    {
        positionComponents.Add(newPosition);
    }
    public void AddSpeedComponent(SpeedComponent newSpeed)
    {
        speedComponents.Add(newSpeed);
    }
}

