using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Map map;

    void Start()
    {
        Application.targetFrameRate = 90;
        map.Generate();
    }

    
    public void Update()
    {
        
    }

}
