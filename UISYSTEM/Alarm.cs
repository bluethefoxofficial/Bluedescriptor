
using Bluedescriptor_Rewritten.Classes;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class Alarm
    {
    GameObject alarm;

    //action to be called when alarm is triggered
    public Action alarm_action;
        public Action Onalarmsoundstop;

      

        AudioSource aud = new AudioSource();

        public Alarm() {
            // Create a parent GameObject to hold the alarm
            GameObject alarmParent = GameObject.Find("AlarmParent");

            // If the parent GameObject doesn't exist, create it
            if (alarmParent == null)
            {
                alarmParent = new GameObject("AlarmParent");
               // DontDestroyOnLoad(alarmParent);
            }

            // Create alarm GameObject as a child of the parent
            GameObject alarm = new GameObject("alarm");
            alarm.transform.parent = alarmParent.transform;

            // Add CoroutineManager component to the alarm GameObject
            alarm.AddComponent<Classes.CoroutineManager>();

        }
        //start function

        public void Start()
        {
            //start alarm loop
            alarm.GetComponent<CoroutineManager>().StartCoroutine(Alarmloop());
        }

        //stop function
        public void Stop()
        {
            //stop alarm loop
            alarm.GetComponent<CoroutineManager>().StopAllCoroutines();
        }

        /* alarm clock functions */
        public IEnumerator Alarmloop()
        {
            MelonLogger.Msg("Alarm setup");
            while (true)
            {
                while (MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "alarm"))
                {

                    int hour = MelonPreferences.GetEntryValue<int>("Bluedescriptor", "alarm_hour");
                    int minute = MelonPreferences.GetEntryValue<int>("Bluedescriptor", "alarm_minute");

                    //get current time
                    int current_hour = DateTime.Now.Hour;
                    int current_minute = DateTime.Now.Minute;

                    //check if current time is equal to alarm time
                    if (current_hour == hour && current_minute == minute)
                    {

                        //trigger alarm action
                        alarm_action();
                        //play alarm sound
                        string assemblyDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                        Audio audio = new Audio();
                        audio.LoadClip(Path.Combine(assemblyDirectory, "bluedescriptor") + "alarm.ogg").ContinueWith(ac =>
                        {
                            aud.clip = ac.Result;
                            aud.loop= true;
                            aud.Play();
                        });
                        //show alarm message
                     //   CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue Descriptor Alarm", $"The alarm was triggered at {hour}:{minute}");
                        //wait 1 minute
                        yield return new WaitForSeconds(60);
                    }
                    yield return null;
                }
            }
        }
    }
}
