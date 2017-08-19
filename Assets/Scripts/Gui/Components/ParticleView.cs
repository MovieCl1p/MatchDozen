using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gui.Components
{
    public class ParticleView : MonoBehaviour
    {
        public void OnStateChenged(bool state)
        {
            Debug.Log(state);
        }
    }
}
