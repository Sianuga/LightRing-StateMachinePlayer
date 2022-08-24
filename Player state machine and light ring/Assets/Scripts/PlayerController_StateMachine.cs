using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


    public class PlayerController_StateMachine : MonoBehaviour
{
    public struct FrameInput
    {
        public float X, Y;
        public bool JumpDown;
        public bool JumpUp;
    }

    public struct RayRange
    {
        public RayRange(float x1, float y1, float x2, float y2, Vector2 dir)
        {
            Start = new Vector2(x1, y1);
            End = new Vector2(x2, y2);
            Dir = dir;
        }

        public readonly Vector2 Start, End, Dir;
    }




    public Vector3 Velocity { get; private set; }
        public FrameInput Input { get; private set; }
        public bool JumpingThisFrame { get; private set; }
        public bool LandingThisFrame { get; private set; }
        public Vector3 RawMovement { get; private set; }
        public bool Grounded => _colDown;

        private float inputX;
        private bool jumpInputStarted=false;
        private bool jumpInputFinished=false;

        public float time=1f;

        
        private Vector3 _lastPosition;
        private float _currentHorizontalSpeed, _currentVerticalSpeed;

 
        private bool _active;
        void Awake() => Invoke(nameof(Activate), 0.5f);
        void Activate() => _active = true;

        private void Start()
        {

        }
        private void Update() {
            if (!_active) return;
            // Calculate velocity
            Velocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;

            GatherInput();
            RunCollisionChecks();

        if(Grounded)
        {
            jumpInputFinished = false;
        }
     

            CalculateWalk(); // Horizontal movement
            CalculateJumpApex(); // Affects fall speed, so calculate before gravity
            CalculateGravity(); // Vertical movement
            CalculateJump(); // Possibly overrides vertical

            MoveCharacter(); // Actually perform the axis movement

        
         
        }
       
    


        #region Gather Input

        private void GatherInput()
    {


      
            if (jumpInputStarted) 
            {

                  jumpInputStarted = false;
                _lastJumpPressed = Time.time;
            }
        }

        #endregion

        #region Collisions

        [Header("COLLISION")] [SerializeField] private Bounds _characterBounds;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private int _detectorCount = 3;
        [SerializeField] private float _detectionRayLength = 0.1f;
        [SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; 

        private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
        private bool _colUp, _colRight, _colDown, _colLeft;

        private float _timeLeftGrounded;


        private void RunCollisionChecks() {
     
            CalculateRayRanged();

      
            LandingThisFrame = false;
            var groundedCheck = RunDetection(_raysDown);
            if (_colDown && !groundedCheck) _timeLeftGrounded = Time.time; 
            else if (!_colDown && groundedCheck) 
           {
            _jumpCounter = 0;
                _coyoteUsable = true; 
                LandingThisFrame = true;
            }

            _colDown = groundedCheck;

         
            _colUp = RunDetection(_raysUp);
            _colLeft = RunDetection(_raysLeft);
            _colRight = RunDetection(_raysRight);

            bool RunDetection(RayRange range) {
                return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _groundLayer));
            }
        }

        private void CalculateRayRanged() {
            var b = new Bounds(transform.position, _characterBounds.size);

            _raysDown = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
            _raysUp = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
            _raysLeft = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
            _raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
        }


        private IEnumerable<Vector2> EvaluateRayPositions(RayRange range) {
            for (var i = 0; i < _detectorCount; i++) {
                var t = (float)i / (_detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
        }

        private void OnDrawGizmos() {
      
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

   
            if (!Application.isPlaying) {
                CalculateRayRanged();
                Gizmos.color = Color.blue;
                foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft }) {
                    foreach (var point in EvaluateRayPositions(range)) {
                        Gizmos.DrawRay(point, range.Dir * _detectionRayLength);
                    }
                }
            }

            if (!Application.isPlaying) return;

 
            Gizmos.color = Color.red;
            var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
            Gizmos.DrawWireCube(transform.position + move, _characterBounds.size);
        }

        #endregion


        #region Walk

        [Header("WALKING")] [SerializeField] private float _acceleration = 90;
        [SerializeField] private float _moveClamp = 13;
        [SerializeField] private float _deAcceleration = 60f;
        [SerializeField] private float _apexBonus = 2;

        private void CalculateWalk() {
            if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft) {
 
                _currentHorizontalSpeed = 0;
            }
        }

        public void walk(float input)
        {
        inputX = input;
            if(input!=0)
            {
                _currentHorizontalSpeed += input * _acceleration * Time.deltaTime;
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);
                var apexBonus = Mathf.Sign(input) * _apexBonus * _apexPoint;
                _currentHorizontalSpeed += apexBonus * Time.deltaTime;
            }
            else
            {
                _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
            }

        }

    public float getCurrentHorizontalSpeed()
    {
        return _currentHorizontalSpeed;
    }

        #endregion

        #region Gravity

        [Header("GRAVITY")] [SerializeField] private float _fallClamp = -40f;
        [SerializeField] private float _minFallSpeed = 80f;
        [SerializeField] private float _maxFallSpeed = 120f;
        private float _fallSpeed;

        private void CalculateGravity() {
            if (_colDown) {

                if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
            }
            else {
 
                var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;

        
                _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

                if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
            }
        }

        #endregion

        #region Jump

        [Header("JUMPING")] [SerializeField] private float _jumpHeight = 30;
        [SerializeField] private float _jumpApexThreshold = 10f;
        [SerializeField] private float _coyoteTimeThreshold = 0.1f;
        [SerializeField] private float _jumpBuffer = 0.1f;
        [SerializeField] private float _jumpEndEarlyGravityModifier = 3;
        [SerializeField] private int _jumpAmount = 1;
        private int _jumpCounter = 0;
        private bool _coyoteUsable;
        private bool _endedJumpEarly = true;
        private float _apexPoint; 
        private float _lastJumpPressed;
        private bool CanUseCoyote => _coyoteUsable && !_colDown && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
        private bool HasBufferedJump => _colDown && _lastJumpPressed + _jumpBuffer > Time.time;

        public float getCurrentVerticalSpeed()
    {
        return _currentVerticalSpeed;
    }

        private void CalculateJumpApex() {
            if (!_colDown) {
           
                _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
            }
            else {
                _apexPoint = 0;
            }
        }

        private void CalculateJump() {
    
            if (jumpInputStarted && CanUseCoyote ) 
            {
                jumpInputStarted = false;
                _currentVerticalSpeed = _jumpHeight;
                _endedJumpEarly = false;
                _coyoteUsable = false;
                _timeLeftGrounded = float.MinValue;
                JumpingThisFrame = true;
            }
            else if(HasBufferedJump)
            {
            jumpInputStarted = false;
            _currentVerticalSpeed = _jumpHeight;
            _endedJumpEarly = false;
            _coyoteUsable = false;
            _timeLeftGrounded = float.MinValue;
            JumpingThisFrame = true;
        }
            else 
            {
                JumpingThisFrame = false;
            }

          


            if (!_colDown && jumpInputFinished && !_endedJumpEarly && Velocity.y > 0)
            {
            jumpInputFinished = false;
            _currentVerticalSpeed = 0;
            _endedJumpEarly = true;
            }

            if (_colUp) 
            {
                if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
            }
        }

    public void jump(bool input)
    {
        if(input)
        {
            _jumpCounter++;
            jumpInputStarted = input;
            jumpInputFinished = false;
        }
    }

    public void jumpFinished(bool input)
    {
        if(input)
        {
            jumpInputStarted = false;
            jumpInputFinished = input;
        }    
    }

    public bool canJump => _jumpCounter <= _jumpAmount;



    public bool canUseCoyote()
    {
        return CanUseCoyote;
    }

    public bool hasBuffered()
    {
        return HasBufferedJump;
    }

    public void resetJumpCounter()
    {
        _jumpCounter = 0;
    }

    public void incrementJumpCounter()
    {
        _jumpCounter++;
    }

        #endregion

        #region Move

        [Header("MOVE")] [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
        private int _freeColliderIterations = 10;


        private void MoveCharacter() {
            var pos = transform.position;
            RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); 
            var move = RawMovement * Time.deltaTime;
            var furthestPoint = pos + move;

            var hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
            if (!hit) {
                transform.position += move;
                return;
            }
       
            var positionToMoveTo = transform.position;
            for (int i = 1; i < _freeColliderIterations; i++) {
           
                var t = (float)i / _freeColliderIterations;
                var posToTry = Vector2.Lerp(pos, furthestPoint, t);

                if (Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer)) {
                    transform.position = positionToMoveTo;

                    if (i == 1) {
                        if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                        var dir = transform.position - hit.transform.position;
                        transform.position += dir.normalized * move.magnitude;
                    }

                    return;
                }

                positionToMoveTo = posToTry;

            }
        }

        #endregion

      

    }
