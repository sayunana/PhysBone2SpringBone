using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRM;

namespace sayunana
{
    public class PhysBone2SpringBone : EditorWindow
    {
        private Editor _editor;

        private Animator root;
        private GameObject rotationTest;
        private bool debug;

        [MenuItem("sayunana/PhysBone2SpringBone")]
        static void Open()
        {
            var window = GetWindow<PhysBone2SpringBone>();
            window.titleContent = new GUIContent("PhysBone2SpringBone");
        }

        /// <Summary>
        /// ウィンドウのパーツを表示する
        /// </Summary>
        void OnGUI()
        {
            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
            textStyle.wordWrap = true;

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.wordWrap = true;

            GUILayout.Label("PhysBone2SpringBone\n" +
                            "このエディターではPhysBoneをSpringBoneに変換します。\n" +
                            "完全な変換ではなく適当な値を代入しているためVRChatと同様の挙動にはなりません。\n" +
                            "完全な変換をするためには手作業での修正が必要になります。", textStyle);

            GUILayout.Space(50);
            root = (Animator)EditorGUILayout.ObjectField("アバターオブジェクト", root, typeof(Animator), true);

            if (root != null)
            {
                //ボタン
                if (GUILayout.Button("PhysBoneをVRMSpringBoneに変換する", buttonStyle))
                {
                    var physBones = GetVRCPhysBones();
                    var physBonesColliders = GetVRCPhysBoneColliders();

                    //変換したコライダーをリスト化
                    Dictionary<VRCPhysBoneColliderBase, VRMSpringBoneColliderGroup> colliderDictionary =
                        new Dictionary<VRCPhysBoneColliderBase, VRMSpringBoneColliderGroup>();

                    GameObject secondary;
                    try
                    {
                        secondary = root.transform.Find("secondary").gameObject;
                    }catch
                    {
                        secondary = new GameObject("secondary");
                        secondary.transform.parent = root.transform;
                    }

                    #region コライダーの変換

                    foreach (var physBoneCollider in physBonesColliders)
                    {
                        VRMSpringBoneColliderGroup vrmSpringBoneColliderGroup;
                        switch (physBoneCollider.shapeType)
                        {
                            case VRCPhysBoneColliderBase.ShapeType.Sphere:
                            {
                                var sphereCollider = new VRMSpringBoneColliderGroup.SphereCollider();

                                //physBoneColliderがアタッチされているオブジェクトを判定
                                //SpringBoneの場合ボーンに追加する必要がありそうなのでrootTransformにSpringBoneをアタッチする
                                if (physBoneCollider.transform == physBoneCollider.rootTransform)
                                {
                                    if (!physBoneCollider.gameObject.TryGetComponent<VRMSpringBoneColliderGroup>(
                                            out vrmSpringBoneColliderGroup))
                                    {
                                        vrmSpringBoneColliderGroup =
                                            physBoneCollider.gameObject.AddComponent<VRMSpringBoneColliderGroup>();
                                        vrmSpringBoneColliderGroup.Colliders =
                                            new VRMSpringBoneColliderGroup.SphereCollider[0];
                                    }

                                    sphereCollider.Offset.x = physBoneCollider.position.x;
                                    sphereCollider.Offset.y = physBoneCollider.position.y;
                                    sphereCollider.Offset.z = physBoneCollider.position.z;
                                }
                                else
                                {
                                    if (!physBoneCollider.rootTransform.gameObject
                                            .TryGetComponent<VRMSpringBoneColliderGroup>(
                                                out vrmSpringBoneColliderGroup))
                                    {
                                        vrmSpringBoneColliderGroup = physBoneCollider.rootTransform.gameObject
                                            .AddComponent<VRMSpringBoneColliderGroup>();
                                        vrmSpringBoneColliderGroup.Colliders =
                                            new VRMSpringBoneColliderGroup.SphereCollider[0];
                                    }

                                    sphereCollider.Offset.x = physBoneCollider.position.x;
                                    sphereCollider.Offset.y = physBoneCollider.position.y;
                                    sphereCollider.Offset.z = physBoneCollider.position.z;
                                }

                                sphereCollider.Radius = physBoneCollider.radius;

                                //コライダーの参照を追加
                                int count = vrmSpringBoneColliderGroup.Colliders.Length;
                                var sphereColliders = new VRMSpringBoneColliderGroup.SphereCollider[count + 1];
                                for (int i = 0; i < count; i++)
                                {
                                    sphereColliders[i] = vrmSpringBoneColliderGroup.Colliders[i];
                                }

                                sphereColliders[count] = sphereCollider;
                                vrmSpringBoneColliderGroup.Colliders = sphereColliders;
                                colliderDictionary.Add(physBoneCollider, vrmSpringBoneColliderGroup);
                            }
                                break;
                            case VRCPhysBoneColliderBase.ShapeType.Capsule:
                            {
                                var sphereCollider1 = new VRMSpringBoneColliderGroup.SphereCollider();
                                var sphereCollider2 = new VRMSpringBoneColliderGroup.SphereCollider();
                                Vector3 centerPos = Vector3.zero;

                                //physBoneColliderがアタッチされているオブジェクトを判定
                                //SpringBoneの場合ボーンに追加する必要がありそうなのでrootTransformにSpringBoneをアタッチする
                                if (physBoneCollider.transform == physBoneCollider.rootTransform)
                                {
                                    if (!physBoneCollider.gameObject.TryGetComponent<VRMSpringBoneColliderGroup>(
                                            out vrmSpringBoneColliderGroup))
                                    {
                                        vrmSpringBoneColliderGroup =
                                            physBoneCollider.gameObject.AddComponent<VRMSpringBoneColliderGroup>();
                                        vrmSpringBoneColliderGroup.Colliders =
                                            new VRMSpringBoneColliderGroup.SphereCollider[0];
                                    }

                                    centerPos.x = physBoneCollider.position.x;
                                    centerPos.y = physBoneCollider.position.y;
                                    centerPos.z = physBoneCollider.position.z;
                                }
                                else
                                {
                                    if (!physBoneCollider.rootTransform.gameObject
                                            .TryGetComponent<VRMSpringBoneColliderGroup>(
                                                out vrmSpringBoneColliderGroup))
                                    {
                                        vrmSpringBoneColliderGroup = physBoneCollider.rootTransform.gameObject
                                            .AddComponent<VRMSpringBoneColliderGroup>();
                                        vrmSpringBoneColliderGroup.Colliders =
                                            new VRMSpringBoneColliderGroup.SphereCollider[0];
                                    }

                                    centerPos.x = physBoneCollider.position.x;
                                    centerPos.y = physBoneCollider.position.y;
                                    centerPos.z = physBoneCollider.position.z;
                                }

                                //SphereColliderでCapsuleColliderの両端を作成
                                //TODO:正確な値の計算式を考える必要があるかも
                                var transform = vrmSpringBoneColliderGroup.transform;
                                var up = transform.up;
                                var physBoneQuaternion = physBoneCollider.rotation;
                                //上
                                sphereCollider1.Offset = centerPos
                                                         + GetParentQuaternion(transform) * (physBoneQuaternion *
                                                             (transform.rotation * (up * physBoneCollider.height / 2)))
                                                         - GetParentQuaternion(transform) * (physBoneQuaternion *
                                                             (transform.rotation * (up * physBoneCollider.radius)));

                                sphereCollider1.Radius = physBoneCollider.radius;

                                //下
                                sphereCollider2.Offset = centerPos
                                                         - GetParentQuaternion(transform) * (physBoneQuaternion *
                                                             (transform.rotation * (up * physBoneCollider.height / 2)))
                                                         + GetParentQuaternion(transform) * (physBoneQuaternion *
                                                             (transform.rotation * (up * physBoneCollider.radius)));

                                sphereCollider2.Radius = physBoneCollider.radius;

                                //コライダーの参照を追加
                                int count = vrmSpringBoneColliderGroup.Colliders.Length;
                                var sphereColliders = new VRMSpringBoneColliderGroup.SphereCollider[count + 2];
                                for (int i = 0; i < count - 1; i++)
                                {
                                    sphereColliders[i] = vrmSpringBoneColliderGroup.Colliders[i];
                                }

                                sphereColliders[count] = sphereCollider1;
                                sphereColliders[count + 1] = sphereCollider2;
                                vrmSpringBoneColliderGroup.Colliders = sphereColliders;
                                colliderDictionary.Add(physBoneCollider, vrmSpringBoneColliderGroup);
                            }
                                break;
                            case VRCPhysBoneColliderBase.ShapeType.Plane:
                            {
                                //PlaneのコライダーはVRMSpringBoneColliderGroupに存在しないので実装しない
                            }
                                break;
                        }
                    }

                    #endregion

                    #region 物理挙動の設定

                    foreach (var physBone in physBones)
                    {
                        var vrmSpringBone = secondary.AddComponent<VRMSpringBone>();

                        //VRCPhysBoneの親Transformを揺らさないようにする設定を反映
                        if (null == physBone.rootTransform)
                        {
                            vrmSpringBone.RootBones.Add(physBone.transform);
                        }
                        else
                        {
                            int childCount = physBone.rootTransform.childCount;
                            for (int i = 0; i < childCount; i++)
                            {
                                Transform childTransform = physBone.rootTransform.GetChild(i);
                                vrmSpringBone.RootBones.Add(childTransform);
                            }
                        }

                        //適当な値でSpringBoneの設定を行う
                        //TODO:正確な値を設定する
                        vrmSpringBone.m_gravityPower = Mathf.Pow(physBone.gravity, 0.5f);
                        vrmSpringBone.m_stiffnessForce = Mathf.Pow(physBone.stiffness, 0.5f);
                        vrmSpringBone.m_dragForce = Mathf.Pow(physBone.pull, 0.5f);

                        if (physBone.colliders.Count != 0)
                        {
                            List<VRMSpringBoneColliderGroup> colliderGroups = new List<VRMSpringBoneColliderGroup>();
                            foreach (var collider in physBone.colliders)
                            {
                                if (collider == null) continue;
                                //PlaneのコライダーはVRMSpringBoneColliderGroupに存在しないのでcolliderGroupsに追加しない
                                if (collider.shapeType == VRCPhysBoneColliderBase.ShapeType.Plane) continue;
                                if (colliderDictionary.ContainsKey(collider))
                                {
                                    colliderGroups.Add(colliderDictionary[collider]);
                                }
                            }

                            vrmSpringBone.ColliderGroups = colliderGroups.ToArray();
                        }
                    }

                    #endregion

                    //PhysBoneとColliderを削除
                    foreach (var physBone in physBones)
                    {
                        DestroyImmediate(physBone);
                    }

                    foreach (var physBonesCollider in physBonesColliders)
                    {
                        DestroyImmediate(physBonesCollider);
                    }
                }

                #region DEBUG

                GUILayout.Space(20);
                debug = GUILayout.Toggle(debug, "DEBUG");
                if (debug)
                {
                    GUILayout.Label("DEBUGの操作は取り消せません");
                    GUILayout.Space(20);
                    if (GUILayout.Button("VRCPhysBoneとVRCPhysBoneColliderをすべて削除する", buttonStyle))
                    {
                        var vrcPhysBones = GetVRCPhysBones();
                        var vrcPhysBoneColliders = GetVRCPhysBoneColliders();

                        foreach (var VRCPhysBone in vrcPhysBones)
                        {
                            DestroyImmediate(VRCPhysBone);
                        }

                        foreach (var VRCPhysBoneCollider in vrcPhysBoneColliders)
                        {
                            DestroyImmediate(VRCPhysBoneCollider);
                        }
                    }

                    GUILayout.Space(10);
                    if (GUILayout.Button("VRMSpringBoneとVRMSpringBoneColliderGroupをすべて削除する", buttonStyle))
                    {
                        var vrmSpringBones = GetVRMSpringBones();
                        var vrmSpringBoneColliderGroups = GetVRMSpringBoneColliderGroups();

                        foreach (var vrmSpringBone in vrmSpringBones)
                        {
                            DestroyImmediate(vrmSpringBone);
                        }

                        foreach (var vrmSpringBoneColliderGroup in vrmSpringBoneColliderGroups)
                        {
                            DestroyImmediate(vrmSpringBoneColliderGroup);
                        }
                    }

                    GUILayout.Space(10);
                    if (GUILayout.Button("VRCPhysBoneとVRCPhysBoneColliderをすべて無効化する", buttonStyle))
                    {
                        var vrcPhysBones = GetVRCPhysBones();
                        var vrcPhysBoneColliders = GetVRCPhysBoneColliders();

                        foreach (var vrcPhysBone in vrcPhysBones)
                        {
                            vrcPhysBone.enabled = false;
                        }

                        foreach (var vrcPhysBoneCollider in vrcPhysBoneColliders)
                        {
                            vrcPhysBoneCollider.enabled = false;
                        }
                    }
                }

                #endregion

                #region 取得系

                VRCPhysBone[] GetVRCPhysBones()
                {
                    return root.gameObject.GetComponentsInChildren<VRCPhysBone>();
                }

                VRCPhysBoneCollider[] GetVRCPhysBoneColliders()
                {
                    return root.gameObject.GetComponentsInChildren<VRCPhysBoneCollider>();
                }

                VRMSpringBone[] GetVRMSpringBones()
                {
                    return root.gameObject.GetComponentsInChildren<VRMSpringBone>();
                }

                VRMSpringBoneColliderGroup[] GetVRMSpringBoneColliderGroups()
                {
                    return root.gameObject.GetComponentsInChildren<VRMSpringBoneColliderGroup>();
                }

                Quaternion GetParentQuaternion(Transform transform)
                {
                    if (transform.parent == null)
                    {
                        return Quaternion.identity;
                    }

                    return transform.parent.rotation * GetParentQuaternion(transform.parent);
                }

                #endregion
            }
        }
    }
}