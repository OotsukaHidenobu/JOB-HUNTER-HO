using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWeaponKind : MonoBehaviour
{
    public WeaponKind weaponKind = WeaponKind.None;
}

public enum WeaponKind
{
    None,
    DefaultGun,
    Shot_Gun,
    Submachine_Gun,
    Circle_Diffusion_Gun,
    DNA_Gun,
    Meteor_Gun,
    Involute_Gun,
    Bullet_To_Bullet,
    PonPon_Gun,
    Stop_And_Go
}