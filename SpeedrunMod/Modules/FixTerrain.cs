using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace SpeedrunMod.Modules {
    [UsedImplicitly]
    public class FixTerrain : FauxMod {

        public override void Initialize() {
            Unload();

            USceneManager.activeSceneChanged += SceneChanged;

            // Modding.ModHooks.Instance.SlashHitHook += LogSlashHit;
        }

        public override void Unload() {
            USceneManager.activeSceneChanged -= SceneChanged;
        }

        // private static void LogSlashHit(Collider2D othercollider, GameObject gameobject) {
        //     Modding.Logger.Log(othercollider.name);
        // }

        // foreach (EdgeCollider2D edgeCollider2D in GameObject.Find("Chunk 1 0").GetComponentsInChildren<EdgeCollider2D>()) {
        //     Modding.Logger.Log("---------------");
        //     Modding.Logger.Log(edgeCollider2D.points.Length);
        //     foreach (Vector2 vector2 in edgeCollider2D.points) {
        //         Modding.Logger.Log(vector2);
        //     }
        // }

        private static void SceneChanged(Scene from, Scene to) {
            switch (to.name) {
                case "Crossroads_21": {
                    HeroController.instance.StartCoroutine(Crossroads_21());
                    break;
                }
                case "Tutorial_01": {
                    HeroController.instance.StartCoroutine(Tutorial_01());
                    break;
                }
                case "Ruins2_06": {
                    HeroController.instance.StartCoroutine(Ruins2_06());
                    break;
                }
                case "RestingGrounds_05": {
                    HeroController.instance.StartCoroutine(RestingGrounds_05());
                    break;
                }
                case "Fungus3_22": {
                    HeroController.instance.StartCoroutine(Fungus3_22());
                    break;
                }
            }
        }

        // cdash to failed champ
        // https://i.imgur.com/w2A9BD5.png
        private static IEnumerator Crossroads_21() {
            yield return null;

            EdgeCollider2D col = GameObject.Find("Chunk 0 0").GetComponentsInChildren<EdgeCollider2D>().First(x => x.points.Length == 24);
            List<Vector2> pts = col.points.ToList();
            pts[4] = new Vector2(29.5f, 10);
            pts[5] = new Vector2(29.5f, 5.7f);
            pts[6] = new Vector2(31, 5.7f);
            col.points = pts.ToArray();
        }

        // kings pass climb to cliffs
        // https://i.imgur.com/HR5aYFY.png
        private static IEnumerator Tutorial_01() {
            yield return null;

            GameObject.Find("Roof Collider (4)").transform.position = new Vector3(5, 59.8f);

            EdgeCollider2D col = GameObject.Find("Chunk 1 0").GetComponentsInChildren<EdgeCollider2D>().First(x => x.points.Length == 14);
            List<Vector2> pts = col.points.ToList();
            pts[9] = new Vector2(3, 29.5f);
            pts[10] = new Vector2(5, 29.5f);
            col.points = pts.ToArray();
        }

        // kings station wall
        // https://i.imgur.com/r1DITlh.png
        private static IEnumerator Ruins2_06() {
            yield return null;

            EdgeCollider2D col = GameObject.Find("Chunk 1 0").GetComponentsInChildren<EdgeCollider2D>().First(x => x.points.Length == 16);
            List<Vector2> pts = col.points.ToList();
            pts.Insert(12, new Vector2(12, 6));
            pts.Insert(13, new Vector2(12, 4.8f));
            pts[14] = new Vector2(13, 4.8f);
            col.points = pts.ToArray();
        }

        // resting grounds seer climb
        // https://i.imgur.com/qamFj3D.png
        private static IEnumerator RestingGrounds_05() {
            yield return null;

            EdgeCollider2D col = GameObject.Find("Chunk 2 0").GetComponentsInChildren<EdgeCollider2D>().First(x => x.points.Length == 9);
            List<Vector2> pts = col.points.ToList();
            pts[1] = new Vector2(24, 7);
            pts.RemoveAt(2);
            col.points = pts.ToArray();
        }

        // qg near traitor lord
        // https://i.imgur.com/7tJ2hxd.png
        // https://i.imgur.com/cBPlRxA.png
        // https://i.imgur.com/3cFccM9.png
        private static IEnumerator Fungus3_22() {
            yield return null;

            EdgeCollider2D col = GameObject.Find("Chunk 0 0").GetComponentsInChildren<EdgeCollider2D>().First(x => x.points[0] == new Vector2(13, 9));
            List<Vector2> pts = col.points.ToList();
            pts[1] = new Vector2(13, 6.6f);
            pts.Insert(2, new Vector2(14, 6.6f));
            pts.Insert(3, new Vector2(14, 8));
            pts.Insert(4, new Vector2(25, 8));
            pts.Insert(5, new Vector2(25, 6.6f));
            pts[6] = new Vector2(26, 6.6f);
            col.points = pts.ToArray();

            EdgeCollider2D col2 = GameObject.Find("Chunk 1 0").GetComponentsInChildren<EdgeCollider2D>().First(x => x.points.Length == 22);
            List<Vector2> pts2 = col2.points.ToList();
            pts2.Insert(18, new Vector2(21, 28));
            pts2.Insert(19, new Vector2(21, 27));
            pts2[20] = new Vector2(22, 27);
            col2.points = pts2.ToArray();

            EdgeCollider2D col3 = GameObject.Find("Chunk 2 0").GetComponentsInChildren<EdgeCollider2D>().First(x => x.points.Length == 16);
            List<Vector2> pts3 = col3.points.ToList();
            pts3[1] = new Vector2(13, 7.6f);
            pts3.Insert(2, new Vector2(14, 7.6f));
            pts3.Insert(3, new Vector2(14, 9));
            col3.points = pts3.ToArray();
        }

    }
}