using System;
using UnityEngine;
using Object = UnityEngine.Object;

[AddComponentMenu("2D Toolkit/UI/Core/tk2dUICircularMask")]
[RequireComponent(typeof (MeshRenderer))]
[RequireComponent(typeof (MeshFilter))]
[ExecuteInEditMode]
public sealed class tk2dUICircularMask : MonoBehaviour
{
    private static readonly Vector2[] Uv =
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(1, 1)
    };

    private static readonly int[] Indices = new int[6]
    {
        0,
        3,
        1,
        2,
        3,
        0
    };

    [SerializeField] private tk2dBaseSprite.Anchor _anchor = tk2dBaseSprite.Anchor.MiddleCenter;

    [SerializeField] private Vector2 _size = new Vector2(1, 1);

    [SerializeField] private float _depth = 1.0f;

    [SerializeField] private float _angle = 180f;

    [SerializeField] private bool _createBoxCollider = true;

    private MeshFilter _thisMeshFilter = null;

    private BoxCollider _thisBoxCollider = null;

    public MeshFilter ThisMeshFilter
    {
        get
        {
            if (_thisMeshFilter == null)
            {
                _thisMeshFilter = GetComponent<MeshFilter>();
            }

            return _thisMeshFilter;
        }
    }


    private BoxCollider ThisBoxCollider
    {
        get
        {
            if (_thisBoxCollider == null)
            {
                _thisBoxCollider = GetComponent<BoxCollider>();
            }

            return _thisBoxCollider;
        }
    }

    

    public void Build()
    {
        if (ThisMeshFilter.sharedMesh == null)
        {
            Mesh mesh = new Mesh();
            mesh.hideFlags = HideFlags.DontSave;
            ThisMeshFilter.mesh = FillMesh(mesh);
        }
        else
        {
            FillMesh(ThisMeshFilter.sharedMesh);
        }

        if (_createBoxCollider)
        {
            if (ThisBoxCollider == null)
            {
                _thisBoxCollider = gameObject.AddComponent<BoxCollider>();
            }
            Bounds bounds = ThisMeshFilter.sharedMesh.bounds;
            ThisBoxCollider.center = new Vector3(bounds.center.x, bounds.center.y, -_depth);
            ThisBoxCollider.size = new Vector3(bounds.size.x, bounds.size.y, 0.0002f);
        }
        else if (ThisBoxCollider != null)
        {
#if UNITY_EDITOR
            Object.DestroyImmediate(ThisBoxCollider);
#else
            Object.Destroy(ThisBoxCollider);
#endif
        }
    }

    public void ReshapeBounds(Vector3 dMin, Vector3 dMax)
    {
        Vector3 oldSize = new Vector3(_size.x, _size.y);
        Vector3 oldMin = Vector3.zero;
        switch (_anchor)
        {
            case tk2dBaseSprite.Anchor.LowerLeft:
                oldMin.Set(0, 0, 0);
                break;
            case tk2dBaseSprite.Anchor.LowerCenter:
                oldMin.Set(0.5f, 0, 0);
                break;
            case tk2dBaseSprite.Anchor.LowerRight:
                oldMin.Set(1, 0, 0);
                break;
            case tk2dBaseSprite.Anchor.MiddleLeft:
                oldMin.Set(0, 0.5f, 0);
                break;
            case tk2dBaseSprite.Anchor.MiddleCenter:
                oldMin.Set(0.5f, 0.5f, 0);
                break;
            case tk2dBaseSprite.Anchor.MiddleRight:
                oldMin.Set(1, 0.5f, 0);
                break;
            case tk2dBaseSprite.Anchor.UpperLeft:
                oldMin.Set(0, 1, 0);
                break;
            case tk2dBaseSprite.Anchor.UpperCenter:
                oldMin.Set(0.5f, 1, 0);
                break;
            case tk2dBaseSprite.Anchor.UpperRight:
                oldMin.Set(1, 1, 0);
                break;
        }
        oldMin = Vector3.Scale(oldMin, oldSize) * -1;
        Vector3 newSize = oldSize + dMax - dMin;
        Vector3 scaledMin = new Vector3(Mathf.Approximately(oldSize.x, 0) ? 0 : (oldMin.x * newSize.x / oldSize.x),
            Mathf.Approximately(oldSize.y, 0) ? 0 : (oldMin.y * newSize.y / oldSize.y));
        Vector3 offset = oldMin + dMin - scaledMin;
        offset.z = 0;
        transform.position = transform.TransformPoint(offset);
        _size = new Vector2(newSize.x, newSize.y);

        Build();
    }

    // Use this for initialization
    private void Awake()
    {
        Build();
    }

    private void OnDestroy()
    {
        if (ThisMeshFilter.sharedMesh != null)
        {
#if UNITY_EDITOR
            Object.DestroyImmediate(ThisMeshFilter.sharedMesh);
#else
            Object.Destroy(ThisMeshFilter.sharedMesh);
#endif
        }
    }

    private Mesh FillMesh(Mesh mesh)
    {
        Vector3 anchorOffset = Vector3.zero;
        switch (_anchor)
        {
            case tk2dBaseSprite.Anchor.UpperLeft:
                anchorOffset = new Vector3(0, -_size.y, 0);
                break;
            case tk2dBaseSprite.Anchor.UpperCenter:
                anchorOffset = new Vector3(-_size.x / 2, -_size.y, 0);
                break;
            case tk2dBaseSprite.Anchor.UpperRight:
                anchorOffset = new Vector3(-_size.x, -_size.y, 0);
                break;
            case tk2dBaseSprite.Anchor.MiddleLeft:
                anchorOffset = new Vector3(0, -_size.y / 2, 0);
                break;
            case tk2dBaseSprite.Anchor.MiddleCenter:
                anchorOffset = new Vector3(-_size.x / 2, -_size.y / 2, 0);
                break;
            case tk2dBaseSprite.Anchor.MiddleRight:
                anchorOffset = new Vector3(-_size.x, -_size.y / 2, 0);
                break;
            case tk2dBaseSprite.Anchor.LowerLeft:
                anchorOffset = new Vector3(0, 0, 0);
                break;
            case tk2dBaseSprite.Anchor.LowerCenter:
                anchorOffset = new Vector3(-_size.x / 2, 0, 0);
                break;
            case tk2dBaseSprite.Anchor.LowerRight:
                anchorOffset = new Vector3(-_size.x, 0, 0);
                break;
        }

        Vector3[] positions = null;
        if (_angle <= 0)
        {
            positions = new Vector3[4]
            {
                anchorOffset + new Vector3(0, 0, -_depth),
                anchorOffset + new Vector3(_size.x, 0, -_depth),
                anchorOffset + new Vector3(0, _size.y, -_depth),
                anchorOffset + new Vector3(_size.x, _size.y, -_depth)
            };
        }
        else if (_angle > 0 && _angle < 90)
        {
            positions = new Vector3[6]
            {
                anchorOffset + new Vector3(0, 0, -_depth),
                anchorOffset + new Vector3(_size.x / 2, _size.y / 2, -_depth),
                anchorOffset + new Vector3(_size.x * Mathf.Cos(_angle / Mathf.PI), _size.x * Mathf.Sin(_angle / Mathf.PI), -_depth),
                anchorOffset + new Vector3(_size.x, 0, -_depth),
                anchorOffset + new Vector3(0, _size.y, -_depth),
                anchorOffset + new Vector3(_size.x, _size.y, -_depth)
            };
        }
        else if (_angle >= 90 && _angle < 180)
        {
            positions = new Vector3[5]
            {
                anchorOffset + new Vector3(0, 0, -_depth),
                anchorOffset + new Vector3(_size.x / 2, _size.y / 2, -_depth),
                anchorOffset + new Vector3(_size.x * Mathf.Cos(_angle / Mathf.PI), _size.x * Mathf.Sin(_angle / Mathf.PI), -_depth),
                anchorOffset + new Vector3(0, _size.y, -_depth),
                anchorOffset + new Vector3(_size.x, _size.y, -_depth)
            };
        }
        else if (_angle >= 180 && _angle < 270)
        {
            positions = new Vector3[4]
            {
                anchorOffset + new Vector3(0, 0, -_depth),
                anchorOffset + new Vector3(_size.x / 2, _size.y / 2, -_depth),
                anchorOffset + new Vector3(_size.x * Mathf.Cos(_angle / Mathf.PI), _size.x * Mathf.Sin(_angle / Mathf.PI), -_depth),
                anchorOffset + new Vector3(_size.x, _size.y, -_depth)
            };
        }
        else if (_angle >= 270)
        {
            positions = new Vector3[3]
            {
                anchorOffset + new Vector3(0, 0, -_depth),
                anchorOffset + new Vector3(_size.x / 2, _size.y / 2, -_depth),
                anchorOffset + new Vector3(_size.x * Mathf.Cos(_angle / Mathf.PI), _size.x * Mathf.Sin(_angle / Mathf.PI), -_depth),
            };
        }

        mesh.vertices = positions;
        mesh.uv = Uv;
        mesh.triangles = Indices;

        // Recalculate bounds, and reset bounds center, i.e. determining draw order for
        // transparent sprites
        Bounds bounds = new Bounds();
        bounds.SetMinMax(anchorOffset, anchorOffset + new Vector3(_size.x, _size.y, 0));
        mesh.bounds = bounds;

        return mesh;
    }

    private void OnDrawGizmosSelected()
    {
        Mesh mesh = ThisMeshFilter.sharedMesh;
        if (mesh != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Bounds bounds = mesh.bounds;
            Gizmos.color = new Color32(56, 146, 227, 96);
            float d = -_depth * 1.001f;
            Vector3 center = new Vector3(bounds.center.x, bounds.center.y, d * 0.5f);
            Vector3 size = new Vector3(bounds.extents.x * 2, bounds.extents.y * 2, Mathf.Abs(d));
            Gizmos.DrawCube(center, size);

            Gizmos.color = new Color32(22, 145, 255, 255);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
