using UnityEngine;

namespace UnityStandardAssets.Effects
{
	public class ExtinguishableParticleSystem : MonoBehaviour
	{
		public float multiplier = 1f;

		private ParticleSystem[] m_Systems;

		private void Start()
		{
			m_Systems = GetComponentsInChildren<ParticleSystem>();
		}

		public void Extinguish()
		{
			ParticleSystem[] systems = m_Systems;
			foreach (ParticleSystem particleSystem in systems)
			{
				var _temp_val_875 = particleSystem.emission; _temp_val_875.enabled = false;
			}
		}
	}
}
