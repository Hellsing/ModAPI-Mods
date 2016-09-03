using System;
using TheForest.Buildings.Creation;
using TheForest.Buildings.World;
using TheForest.World;
using UnityEngine;

namespace GriefClientPro.KeyActions
{
    public class SphereAction
    {
        public static class Enabled
        {
            public static bool Trees = true;
            public static bool TreeStumps = true;
            public static bool SuitCases = true;
            public static bool BluePrints = true;
            public static bool BreakableCrates = true;
            public static bool Buildings = true;
            public static bool Bushes = true;
            public static bool KillPlayers;
        }

        private readonly GameObject _sphere;
        private float _sphereRadius = 10;

        private bool _executeActions;

        public SphereAction()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var mf = go.GetComponent<MeshFilter>();
            var mesh = mf.sharedMesh;

            _sphere = new GameObject { name = "Inverted Sphere" };
            var mfNew = _sphere.AddComponent<MeshFilter>();
            mfNew.sharedMesh = new Mesh();
            //Scale the vertices;
            var vertices = mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertices[i];
            }
            mfNew.sharedMesh.vertices = vertices;

            // Reverse the triangles
            var triangles = mesh.triangles;
            for (var i = 0; i < triangles.Length; i += 3)
            {
                var t = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = t;
            }
            mfNew.sharedMesh.triangles = triangles;

            // Reverse the normals;
            var normals = mesh.normals;
            for (var i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mfNew.sharedMesh.normals = normals;

            mfNew.sharedMesh.uv = mesh.uv;
            mfNew.sharedMesh.uv2 = mesh.uv2;
            mfNew.sharedMesh.RecalculateBounds();

            UnityEngine.Object.DestroyImmediate(go);

            _sphere.AddComponent<MeshRenderer>();
            _sphere.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
            _sphere.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.9f));
            _sphere.GetComponent<MeshRenderer>().enabled = false;
        }

        public void OnPrepare()
        {
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                _sphereRadius = Mathf.Clamp(_sphereRadius + Input.mouseScrollDelta.y, 10f, 300f);
            }
            _sphere.GetComponent<MeshRenderer>().enabled = true;
            _sphere.transform.position = Camera.main.transform.position;
            _sphere.transform.localScale = new Vector3(_sphereRadius * 2f, _sphereRadius * 2f, _sphereRadius * 2f);
            _executeActions = true;
        }

        public void OnTick()
        {
            if (_executeActions)
            {
                ExecuteAroundPosition(_sphere.transform.position, _sphereRadius);

                _sphere.GetComponent<MeshRenderer>().enabled = false;
                _executeActions = false;
            }
        }

        public static void ExecuteAroundPosition(Vector3 position, float radius)
        {
            try
            {
                var hits = Physics.SphereCastAll(position, radius, new Vector3(1f, 0f, 0f));
                foreach (var hit in hits)
                {
                    if (Enabled.Buildings && hit.collider.GetComponent<destroyStructure>() != null)
                    {
                        var structure = hit.collider.GetComponent<destroyStructure>();
                        structure.SendMessage("Hit", structure.health);
                    }
                    else if (Enabled.SuitCases && hit.collider.GetComponent<SuitCase>() != null)
                    {
                        hit.collider.gameObject.SendMessage("Open");
                        hit.collider.gameObject.SendMessage("Open_Perform");
                    }
                    else if (Enabled.Trees && hit.collider.GetComponent<TreeHealth>() != null)
                    {
                        hit.collider.gameObject.SendMessage("Explosion", 100f);
                    }
                    else if (Enabled.BluePrints && hit.collider.GetComponent<Craft_Structure>() != null)
                    {
                        hit.collider.gameObject.SendMessage("CancelBlueprint");
                        hit.collider.gameObject.SendMessage("CancelBlueprintSafe");
                    }
                    else if (Enabled.BreakableCrates && hit.collider.GetComponent<BreakCrate>() != null)
                    {
                        hit.collider.gameObject.SendMessage("Explosion");
                    }
                    else if (Enabled.Buildings && hit.collider.GetComponent<BuildingExplosion>() != null)
                    {
                        hit.collider.gameObject.SendMessage("UnlocalizedExplode");
                    }
                    else if (Enabled.KillPlayers && hit.collider.GetComponent<CoopPlayerRemoteSetup>() != null)
                    {
                        KillAllPlayers.KillSinglePlayer(hit.collider.GetComponent<CoopPlayerRemoteSetup>());
                    }
                    else if (Enabled.BreakableCrates && hit.collider.GetComponent<BreakWoodSimple>() != null)
                    {
                        hit.collider.gameObject.SendMessage("Hit", 10000);
                    }
                    else if (Enabled.Buildings && hit.collider.GetComponent<Fire2>() != null)
                    {
                        hit.collider.gameObject.SendMessage("DestroyFire");
                    }
                    else if (Enabled.Buildings && (hit.collider.GetComponent<BuildingHealthHitRelay>() != null ||
                                                   hit.collider.GetComponent<BuildingHealthChunkHitRelay>() != null ||
                                                   hit.collider.GetComponent<FoundationChunkTier>() != null ||
                                                   hit.collider.GetComponent<BuildingHealth>() != null ||
                                                   hit.collider.GetComponent<BreakWoodSimple>() != null) ||
                             Enabled.TreeStumps && hit.collider.GetComponent<ExplodeTreeStump>() != null)
                    {
                        hit.collider.gameObject.SendMessage("LocalizedHit", new LocalizedHitData(hit.collider.gameObject.transform.position, 1000f));
                    }
                    else if (Enabled.Bushes && (hit.collider.GetComponent<CutSappling>() != null ||
                                                hit.collider.GetComponent<CutBush>() != null ||
                                                hit.collider.GetComponent<CutBush2>() != null ||
                                                hit.collider.GetComponent<CutEffigy>() != null ||
                                                hit.collider.GetComponent<CutStalactite>() != null))
                    {
                        hit.collider.gameObject.SendMessage("CutDown");
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
