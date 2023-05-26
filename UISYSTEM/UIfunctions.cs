using ABI_RC.Core.UI;
using Bluedescriptor_Rewritten.Classes;
using BTKUILib.UIObjects.Components;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bluedescriptor_Rewritten.UISYSTEM
{
    internal class UIfunctions
    {
        public void panic()
        {
            
            CohtmlHud.Instance.ViewDropTextImmediate($"<color=blue>[BD]</color>", $"Blue descriptor safety","Panic was initilized reload all avatars to undo.");
            //get all gameobjects with the CVRAvatar component from all scenes
            GameObject[] players = new CVRPlayer().remotePlayers();
            //loop through all players
            foreach (var player in players)
            {
                MelonLogger.Msg("Logging Renderers: "+player);
                //get all renderers from the player
                Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
                //loop through all renderers
                foreach (var renderer in renderers)
                {
                    //get all materials from the renderer
                    Material[] materials = renderer.materials;
                    //loop through all materials
                    foreach (var material in materials)
                    {
                        //remove the shader from the material and replace it with a standard shader
                        material.shader = Shader.Find("Standard");
                    }
                }

                //meshrenderers
                MeshRenderer[] meshrenderers = player.GetComponentsInChildren<MeshRenderer>();
                foreach (var meshrenderer in meshrenderers)
                {
                    MelonLogger.Msg("Logging mesh renderers: " + player);
                    Material[] materials = meshrenderer.materials;
                    foreach (var material in materials)
                    {
                        material.shader = Shader.Find("Standard");
                    }
                }
            }
        }
 
    }
}
