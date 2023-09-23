using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPill : MonoBehaviour
{
    #region Editor Varibles
    [SerializeField]
    [Tooltip("How much faster this pill makes you move")]
    private int m_SpeedGain;
    public int SpeedGain
    {
        get
        {
            return m_SpeedGain;
        }
    }

    [SerializeField]
    [Tooltip("How long this effect lasts")]
    private float m_EffectLength;
    public float EffectLength
    {
        get
        {
            return m_EffectLength;
        }
    }
    #endregion
}
