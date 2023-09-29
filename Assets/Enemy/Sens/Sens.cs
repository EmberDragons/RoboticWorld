using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class Sens : MonoBehaviour
{
    //sens
    //view
    Transform viewPoint;
    public float viewAngle;
    public float viewRange;
    //hear
    Transform hearPoint;
    public float hearRange;
    //smell
    Transform smellPoint;
    public float smellRange;
    //Food
    public float seekRange;
    //Layers
    public LayerMask _alive;
    public LayerMask _smells;
    public LayerMask _environnement;
    public LayerMask _food;
    public LayerMask _Interest;
    //Targets
    public Transform _viewTarget;
    public Transform _hearTarget;
    public Transform _smellTarget;
    public Transform _seekTarget;
    public Transform _placeTarget = null;
    public Transform _feedTarget;

    public List<Transform> InterestingVectors;
    
    Transform noiseToInvestigate;
    bool followNoise = false;
    private void Awake()
    {
        viewPoint = this.transform.Find("Body/Head/Eyes");
        hearPoint = this.transform.Find("Body/Head/Ears");
        smellPoint = this.transform.Find("Body/Head/Nose");
    }
    void Start()
    {
        StartCoroutine(LookTime());
        StartCoroutine(HearTime());
        StartCoroutine(SmellTime());
        StartCoroutine(SeekTime());
        StartCoroutine(PlaceTime());
        StartCoroutine(FeedTime());
        StartCoroutine(PlaceTime());
        _viewTarget = null;
        _hearTarget = null;
        _smellTarget = null;
        _seekTarget = null;
        _placeTarget = null;
        _feedTarget = null;
        noiseToInvestigate = new GameObject($"PointToInvestigate {GetComponent<Brain>().Name}").transform;
    }
    void Update()
    {
        if (noiseToInvestigate.position.x + 1.5f >= transform.position.x && noiseToInvestigate.position.x - 1.5f <= transform.position.x)
        {
            if (noiseToInvestigate.position.y + 1.5f >= transform.position.y && noiseToInvestigate.position.y - 1.5f <= transform.position.y)
            {
                followNoise = false;
                if (_hearTarget == noiseToInvestigate.transform) _hearTarget = null;
                noiseToInvestigate.position = new Vector2(0f, 0f);
            }
        }
        if (_hearTarget != null && _hearTarget.GetComponent<Brain>() != null && _hearTarget.GetComponent<Brain>().noisy == false)
        {
            noiseToInvestigate.position = new Vector3(_hearTarget.position.x, _hearTarget.position.y, 0f);
            _hearTarget = noiseToInvestigate;
            followNoise = true;
        }
    }
    
    private IEnumerator LookTime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (true)
        {
            yield return wait;
            Look();
        }
    }

    private IEnumerator HearTime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            Hear();
        }
    }
    private IEnumerator SmellTime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while(true)
        {
            yield return wait;
            Smell();
        }
    }
    private IEnumerator SeekTime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            Seek();
        }
    }
    private IEnumerator PlaceTime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        while (true)
        {
            yield return wait;
            InterestingPoints();
        }
    }
    private IEnumerator FeedTime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            LookForFood();
        }
    }


    void Look()
    {
        Collider2D[] _seeing = Physics2D.OverlapCircleAll(viewPoint.position, viewRange, _alive);
        Vector2 LookDirection = transform.right;
        if (transform.localScale.x < 0f)
        {
            LookDirection = -transform.right;
        }
        if (_seeing.Length > 0)
        {
            int i = _seeing.Length - 1;
            Transform _ourTarget;
            float attractive = 1000f;
            bool look = true;
            while (i >= 0)
            {
                if (_seeing[i].transform == _viewTarget && _viewTarget != null) look = false;
                if (i == 0 && look == true) _viewTarget = null;
                look = true;
                _viewTarget = null;
                i--;
            }
            i = _seeing.Length - 1;
            while (i >= 0 && look == true)
            {
                _ourTarget = _seeing[i].transform;
                float _ourDistance = Vector2.Distance(_ourTarget.position, viewPoint.position);
                Vector2 DirectionTarget = (_ourTarget.position - viewPoint.position).normalized;
                if (_ourTarget.gameObject.GetComponent<Brain>().Name != this.GetComponent<Brain>().Name)
                {
                    if (Vector2.Angle(LookDirection, DirectionTarget) < viewAngle / 2)
                    {
                        float DistanceTarget = Vector2.Distance(viewPoint.position, _ourTarget.position);
                        float Overallrank = _ourTarget.GetComponent<Brain>().Size + _ourTarget.GetComponent<Brain>()._Live + _ourTarget.GetComponent<Brain>().attackDamage + _ourDistance;
                        if (!Physics2D.Raycast(viewPoint.position, DirectionTarget, DistanceTarget, _environnement) && Overallrank < attractive && _ourTarget.gameObject.GetComponent<Brain>()._Live != 0f)
                        {
                            attractive = Overallrank;
                            _viewTarget = _ourTarget.transform;
                        }
                        else _viewTarget = null;
                    }
                    else if(Vector2.Distance(_ourTarget.transform.position,transform.position)<= 5f)
                        _viewTarget = _ourTarget.transform;
                }
                i--;
            }
        } 
    }
    void Seek()
    {
        Collider2D[] _seeking = Physics2D.OverlapCircleAll(smellPoint.position, seekRange, _food);
        if (_seeking.Length > 0)
        {
            int i = _seeking.Length - 1;
            Transform _ourTarget = null;
            while (i >= 0)
            {
                if (this.GetComponent<Brain>().TypeOfEater == "carnivor" && _seeking[i].GetComponent<Brain>() != null)
                {
                    if (_seeking[i].GetComponent<Brain>().FoodWorth != 0) _ourTarget = _seeking[i].transform;
                }
                if (this.GetComponent<Brain>().TypeOfEater == "herbivor" && _seeking[i].GetComponent<VegetableFood>() != null)
                {
                    if (_seeking[i].GetComponent<VegetableFood>().FoodWorth != 0) _ourTarget = _seeking[i].transform;
                }

                i--;
                if (i < 0 && _ourTarget != null) _seekTarget = _ourTarget.transform;
            }
        }
    }

    void Hear()
    {
        //gonna use it when chase or see = null
        Collider2D[] _hearing = Physics2D.OverlapCircleAll(hearPoint.position, hearRange, _alive);
        if (_hearing.Length > 0)
        {
            int i = _hearing.Length - 1;
            Transform _ourTarget;
            float attractive = 1000f; 
            bool hear = true;
            while (i >= 0)
            {
                if (_hearing[i].transform == _hearTarget) hear = false;
                i--;
            }
            i = _hearing.Length - 1;
            while (i >= 0 && hear == true)
            {
                _ourTarget = _hearing[i].transform;
                if (_ourTarget.gameObject.GetComponent<Brain>().noisy)
                {
                    float _ourDistance = Vector2.Distance(_ourTarget.position, hearPoint.position);
                    float Overallrank = _ourTarget.GetComponent<Brain>().Size + _ourTarget.GetComponent<Brain>()._Live + _ourTarget.GetComponent<Brain>().attackDamage + _ourDistance;
                    if (attractive > Overallrank && _ourTarget.gameObject.GetComponent<Brain>().Name != this.GetComponent<Brain>().Name && _ourTarget.gameObject.GetComponent<Brain>()._Live > 0f)
                    {
                        attractive = Overallrank;
                        _hearTarget = _ourTarget.transform;
                    }
                }
                i--;
            }
        }
    }
    void Smell()
    {
        //gonna use it when chase or see = null
        Collider2D[] _smelling = Physics2D.OverlapCircleAll(smellPoint.position, smellRange, _smells);
        if (_smelling.Length > 0)
        {
            int i = _smelling.Length - 1;
            Transform _ourTarget;
            float attractive = 999999f; 
            bool smell = true;
            while (i >= 0)
            {
                if (_smelling[i].transform == _smellTarget) smell = false; 
                if (i == 0 && smell == true) _smellTarget = null;
                attractive = 999999f;
                i--;
            }
            i = _smelling.Length - 1;
            while (i >= 0 && smell == true && _smelling[i]!= null)
            {
                if (_smelling[i].GetComponent<publicsmellInfo>().Name != this.GetComponent<Brain>().Name)
                {
                    _ourTarget = _smelling[i].transform;
                    float _ourDistance = Vector2.Distance(_ourTarget.position, viewPoint.position);
                    float Overallrank = _ourTarget.GetComponent<publicsmellInfo>().Size + _ourTarget.GetComponent<publicsmellInfo>().attackDamage + _ourTarget.GetComponent<publicsmellInfo>()._Live + _ourDistance;
                    if (attractive > Overallrank)
                    {
                        attractive = Overallrank;
                        if(_ourTarget != null) _smellTarget = _ourTarget.transform;
                        _smellTarget.GetComponent<publicsmellInfo>().smelledBy = true;
                    }
                }
                i--;
            }
        }
    }
    void InterestingPoints()
    {
        if(_placeTarget == null) {
            Collider2D[] _place = Physics2D.OverlapCircleAll(transform.position, 50f, _Interest);
            int i = -1;
            if (_place.Length > 0)
            {
                for (int j = 0; j < _place.Length; j++)
                {
                    bool interest = true;
                    for (int l = 0; l < InterestingVectors.Count; l++)
                    {
                        if (_place[j].transform == InterestingVectors[l].transform) interest = false;
                        if (l == InterestingVectors.Count - 1 && interest) i = j;
                    }
                    if(InterestingVectors.Count == 0) i = j;
                    if (i != -1) j = _place.Length;
                }
                if (i == -1) InterestingVectors.Clear();
                else
                {
                    _placeTarget = _place[i].transform;
                }
            }
        }
        else
        {
            Collider2D[] _CheckForplace = Physics2D.OverlapCircleAll(transform.position, 2f, _Interest);
            bool right = false;
            if(_CheckForplace.Length > 0)
            {
                for (int l = 0; l < InterestingVectors.Count; l++)
                {
                    if (_CheckForplace[0].transform == InterestingVectors[l].transform) right = true;
                }
                if (right == false)
                {
                    InterestingVectors.Add(_CheckForplace[0].transform);
                    if (_CheckForplace.Length > 0) _placeTarget = null;
                }
            }
        }
    }
    void LookForFood()
    {
        //gonna use it when chase or see = null
        Collider2D[] _feed = Physics2D.OverlapCircleAll(viewPoint.position, smellRange, _food);
        if (_feed.Length > 0)
        {
            int i = _feed.Length-1;
            Transform _ourTarget = null;
            while (i >= 0)
            {
                if (this.GetComponent<Brain>().TypeOfEater == "carnivor" && _feed[i].GetComponent<Brain>() != null)
                {
                    if(_feed[i].GetComponent<Brain>().FoodWorth != 0) _ourTarget = _feed[i].transform;
                }
                if (this.GetComponent<Brain>().TypeOfEater == "herbivor" && _feed[i].GetComponent<VegetableFood>() != null)
                {
                    if (_feed[i].GetComponent<VegetableFood>().FoodWorth != 0) _ourTarget = _feed[i].transform;
                }
                 
                i--;
                if(i < 0 && _ourTarget != null) _feedTarget = _ourTarget.transform;
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (viewPoint != null)
        {
            Gizmos.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(viewPoint.position, Vector3.forward, viewRange);
            if (_viewTarget != null)
            {
                Gizmos.color = Color.green; Gizmos.DrawLine(viewPoint.position, _viewTarget.transform.position);
            }
        }

        if (hearPoint != null)
        {
            Gizmos.color = Color.blue;
            UnityEditor.Handles.DrawWireDisc(hearPoint.position, Vector3.forward, hearRange);
            if (_hearTarget != null && _viewTarget == null)
            {
                Gizmos.color = Color.green; Gizmos.DrawLine(viewPoint.position, _hearTarget.transform.position);
            }
        }

        if(smellPoint != null)
        {
            Gizmos.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(smellPoint.position, Vector3.forward, smellRange);
            Vector3 angle01 = DirectionFromAngleView(-viewPoint.eulerAngles.z, -viewAngle / 2);
            Vector3 angle02 = DirectionFromAngleView(-viewPoint.eulerAngles.z, viewAngle / 2);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(viewPoint.position, viewPoint.position + angle01 * viewRange);
            Gizmos.DrawLine(viewPoint.position, viewPoint.position + angle02 * viewRange);
            if (_smellTarget != null && _hearTarget == null && _viewTarget == null)
            {
                Gizmos.color = Color.green; Gizmos.DrawLine(smellPoint.position, _smellTarget.transform.position);
            }
        }
    }

    private Vector2 DirectionFromAngleView(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        if (transform.rotation.y != 0f)
            angleInDegrees = angleInDegrees * (Mathf.Abs(transform.rotation.y)*-1);
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
