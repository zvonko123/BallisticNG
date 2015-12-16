using UnityEngine;
using System.Collections;

public class Medal : MonoBehaviour {

    public E_MEDALTYPE type = E_MEDALTYPE.NONE;
}

public enum E_MEDALTYPE
{
    NONE,
    BRONZE,
    SILVER,
    GOLD
}
