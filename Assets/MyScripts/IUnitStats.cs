using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitStats
{
    public int Strength {  get; set; }
    public int Dexterity {  get; set; }
    public int Endurance {  get; set; }
    public int Experience {  get; set; }
}