using System.Collections.Generic;
using UnityEngine;

namespace SF {
    [CreateAssetMenu(menuName = "SF/Systems/Party")]
    public class Party : ScriptableObject {
        public List<Character> characters;
    }
}
