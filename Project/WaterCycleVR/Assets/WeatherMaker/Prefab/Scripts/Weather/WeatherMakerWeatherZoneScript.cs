using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerWeatherZoneScript : MonoBehaviour
    {
        public WeatherMakerProfileScript WeatherProfile;

        private void OnTriggerEnter(Collider other)
        {
            // start weather
        }

        private void OnTriggerExit(Collider other)
        {
            // stop weather
        }

        private void Awake()
        {
            
        }

        private void Start()
        {
            
        }
    }
}
