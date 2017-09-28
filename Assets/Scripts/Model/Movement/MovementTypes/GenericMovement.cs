﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public enum ManeuverSpeed
    {
        AdditionalMovement,
        Speed0,
        Speed1,
        Speed2,
        Speed3,
        Speed4,
        Speed5
    }

    public enum ManeuverDirection
    {
        Left,
        Forward,
        Right
    }

    public enum ManeuverBearing
    {
        Straight,
        Bank,
        Turn,
        KoiogranTurn,
        SegnorsLoop,
        TallonRoll,
        Stationary,
        Reverse
    }

    public enum ManeuverColor
    {
        None,
        Green,
        White,
        Red
    }

    public struct MovementStruct
    {
        public ManeuverSpeed Speed;
        public ManeuverDirection Direction;
        public ManeuverBearing Bearing;
        public ManeuverColor ColorComplexity;

        public MovementStruct(string parameters)
        {
            string[] arrParameters = parameters.Split('.');

            ManeuverSpeed speed = ManeuverSpeed.Speed1;

            switch (arrParameters[0])
            {
                case "0":
                    speed = ManeuverSpeed.Speed0;
                    break;
                case "1":
                    speed = ManeuverSpeed.Speed1;
                    break;
                case "2":
                    speed = ManeuverSpeed.Speed2;
                    break;
                case "3":
                    speed = ManeuverSpeed.Speed3;
                    break;
                case "4":
                    speed = ManeuverSpeed.Speed4;
                    break;
                case "5":
                    speed = ManeuverSpeed.Speed5;
                    break;
            }

            ManeuverDirection direction = ManeuverDirection.Forward;

            switch (arrParameters[1])
            {
                case "F":
                    direction = ManeuverDirection.Forward;
                    break;
                case "L":
                    direction = ManeuverDirection.Left;
                    break;
                case "R":
                    direction = ManeuverDirection.Right;
                    break;
            }

            ManeuverBearing bearing = ManeuverBearing.Straight;

            switch (arrParameters[2])
            {
                case "S":
                    bearing = (speed != ManeuverSpeed.Speed0) ? ManeuverBearing.Straight : ManeuverBearing.Stationary;
                    break;
                case "R":
                    bearing = (direction == ManeuverDirection.Forward) ? ManeuverBearing.KoiogranTurn : ManeuverBearing.SegnorsLoop;
                    break;
                case "E":
                    bearing = ManeuverBearing.TallonRoll;
                    break;
                case "B":
                    bearing = ManeuverBearing.Bank;
                    break;
                case "T":
                    bearing = ManeuverBearing.Turn;
                    break;
            }

            Speed = speed;
            Direction = direction;
            Bearing = bearing;

            if (!Selection.ThisShip.Maneuvers.ContainsKey(parameters)) Debug.Log("ERROR: Ship doesn't have required maneuver. Seems that AI maneuver table is wrong.");

            ColorComplexity = Selection.ThisShip.Maneuvers[parameters];

            ColorComplexity = Selection.ThisShip.GetColorComplexityOfManeuver(this);
        }

        public int SpeedInt
        {
            set
            {
                ManeuverSpeed speed = ManeuverSpeed.Speed1;
                switch (value)
                {
                    case 0:
                        speed = ManeuverSpeed.Speed0;
                        break;
                    case 1:
                        speed = ManeuverSpeed.Speed1;
                        break;
                    case 2:
                        speed = ManeuverSpeed.Speed2;
                        break;
                    case 3:
                        speed = ManeuverSpeed.Speed3;
                        break;
                    case 4:
                        speed = ManeuverSpeed.Speed4;
                        break;
                    case 5:
                        speed = ManeuverSpeed.Speed5;
                        break;
                    default:
                        break;
                }
                Speed = speed;
            }

            get
            {
                int speed = 0;
                switch (Speed)
                {
                    case ManeuverSpeed.AdditionalMovement:
                        break;
                    case ManeuverSpeed.Speed0:
                        speed = 0;
                        break;
                    case ManeuverSpeed.Speed1:
                        speed = 1;
                        break;
                    case ManeuverSpeed.Speed2:
                        speed = 2;
                        break;
                    case ManeuverSpeed.Speed3:
                        speed = 3;
                        break;
                    case ManeuverSpeed.Speed4:
                        speed = 4;
                        break;
                    case ManeuverSpeed.Speed5:
                        speed = 5;
                        break;
                    default:
                        break;
                }
                return speed;
            }
        }

        public override string ToString()
        {
            string maneuverString = "";

            maneuverString += SpeedInt + ".";

            switch (Direction)
            {
                case ManeuverDirection.Left:
                    maneuverString += "L.";
                    break;
                case ManeuverDirection.Forward:
                    maneuverString += "F.";
                    break;
                case ManeuverDirection.Right:
                    maneuverString += "R.";
                    break;
                default:
                    break;
            }

            switch (Bearing)
            {
                case ManeuverBearing.Straight:
                    maneuverString += "S";
                    break;
                case ManeuverBearing.Bank:
                    maneuverString += "B";
                    break;
                case ManeuverBearing.Turn:
                    maneuverString += "T";
                    break;
                case ManeuverBearing.KoiogranTurn:
                    maneuverString += "R";
                    break;
                case ManeuverBearing.SegnorsLoop:
                    maneuverString += "R";
                    break;
                case ManeuverBearing.TallonRoll:
                    maneuverString += "E";
                    break;
                case ManeuverBearing.Stationary:
                    maneuverString += "S";
                    break;
                default:
                    break;
            }

            return maneuverString;
        }
    }

    public abstract class GenericMovement
    {
        public ManeuverSpeed ManeuverSpeed { get; set; }
        public int Speed { get; set; }
        public ManeuverDirection Direction { get; set; }
        public ManeuverBearing Bearing { get; set; }
        public ManeuverColor ColorComplexity { get; set; }

        protected float ProgressTarget { get; set; }
        protected float ProgressCurrent { get; set; }

        protected float AnimationSpeed { get; set; }

        public MovementPrediction movementPrediction;

        public GenericMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color)
        {
            Speed = speed;
            ManeuverSpeed = GetManeuverSpeed(speed);
            Direction = direction;
            Bearing = bearing;
            ColorComplexity = color;
        }

        public virtual void Initialize()
        {
            ProgressTarget = SetProgressTarget();
            AnimationSpeed = Options.ManeuverSpeed * SetAnimationSpeed();
        }

        protected virtual float SetProgressTarget() { return 0; }

        protected virtual float SetAnimationSpeed() { return 0; }

        private ManeuverSpeed GetManeuverSpeed(int speed)
        {
            ManeuverSpeed maneuverSpeed = ManeuverSpeed.Speed1;

            switch (speed)
            {
                case 0:
                    maneuverSpeed = ManeuverSpeed.Speed0;
                    break;
                case 1:
                    maneuverSpeed = ManeuverSpeed.Speed1;
                    break;
                case 2:
                    maneuverSpeed = ManeuverSpeed.Speed2;
                    break;
                case 3:
                    maneuverSpeed = ManeuverSpeed.Speed3;
                    break;
                case 4:
                    maneuverSpeed = ManeuverSpeed.Speed4;
                    break;
                case 5:
                    maneuverSpeed = ManeuverSpeed.Speed5;
                    break;
            }

            return maneuverSpeed;
        }

        public virtual void Perform()
        {
            ProgressCurrent = 0f;
        }

        public virtual void LaunchShipMovement()
        {
            Selection.ThisShip.StartMoving(LaunchShipMovementContinue);
        }

        private void LaunchShipMovementContinue()
        {
            Selection.ThisShip.ShipsBumped.AddRange(movementPrediction.ShipsBumped);
            foreach (var bumpedShip in Selection.ThisShip.ShipsBumped)
            {
                if (!bumpedShip.ShipsBumped.Contains(Selection.ThisShip))
                {
                    bumpedShip.ShipsBumped.Add(Selection.ThisShip);
                }
            }

            Selection.ThisShip.IsLandedOnObstacle = movementPrediction.IsLandedOnAsteroid;

            if (movementPrediction.AsteroidsHit.Count > 0)
            {
                Selection.ThisShip.ObstaclesHit.AddRange(movementPrediction.AsteroidsHit);
            }

            if (movementPrediction.MinesHit.Count > 0)
            {
                Selection.ThisShip.MinesHit.AddRange(movementPrediction.MinesHit);
            }

            Sounds.PlayFly();
            AdaptSuccessProgress();

            LaunchSimple();
        }

        public virtual void LaunchSimple()
        {
            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.isMoving = true;
        }

        public virtual void UpdateMovementExecution()
        {
            CheckFinish();
        }

        public abstract void AdaptSuccessProgress();

        protected virtual void CheckFinish()
        {
            if (ProgressCurrent == ProgressTarget)
            {
                FinishMovement();
            }
        }

        protected virtual void FinishMovement()
        {
            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Movement.isMoving = false;

            Selection.ThisShip.ResetRotationHelpers();

            ManeuverEndRotation(FinishMovementEvents);
        }

        protected virtual void ManeuverEndRotation(Action callBack)
        {
            callBack();
        }

        protected virtual void FinishMovementEvents()
        { 
            MovementTemplates.HideLastMovementRuler();

            Selection.ThisShip.CallExecuteMoving();

            //Called as callbacks
            //Selection.ThisShip.FinishMovement();
            //Selection.ThisShip.FinishPosition();
            //Phases.FinishSubPhase(typeof(SubPhases.MovementExecutionSubPhase));
        }

        //TODO: Rework
        protected float GetMovement1()
        {
            float result = Board.BoardManager.GetBoard().TransformVector(new Vector3(4, 0, 0)).x;
            return result;
        }

        public abstract GameObject[] PlanMovement();

        public override string ToString()
        {
            string maneuverString = "";

            maneuverString += Speed + ".";

            switch (Direction)
            {
                case ManeuverDirection.Left:
                    maneuverString += "L.";
                    break;
                case ManeuverDirection.Forward:
                    maneuverString += "F.";
                    break;
                case ManeuverDirection.Right:
                    maneuverString += "R.";
                    break;
                default:
                    break;
            }

            switch (Bearing)
            {
                case ManeuverBearing.Straight:
                    maneuverString += "S";
                    break;
                case ManeuverBearing.Bank:
                    maneuverString += "B";
                    break;
                case ManeuverBearing.Turn:
                    maneuverString += "T";
                    break;
                case ManeuverBearing.KoiogranTurn:
                    maneuverString += "R";
                    break;
                case ManeuverBearing.SegnorsLoop:
                    maneuverString += "R";
                    break;
                case ManeuverBearing.TallonRoll:
                    maneuverString += "E";
                    break;
                case ManeuverBearing.Stationary:
                    maneuverString += "S";
                    break;
                default:
                    break;
            }

            return maneuverString;
        }

        public string GetBearingChar()
        {
            string result = "";

            switch (Bearing)
            {
                case ManeuverBearing.Straight:
                    result = "8";
                    break;
                case ManeuverBearing.Bank:
                    result = (Direction == ManeuverDirection.Left) ? "7" : "9";
                    break;
                case ManeuverBearing.Turn:
                    result = (Direction == ManeuverDirection.Left) ? "4" : "6";
                    break;
                case ManeuverBearing.KoiogranTurn:
                    result = "2";
                    break;
                case ManeuverBearing.SegnorsLoop:
                    result = (Direction == ManeuverDirection.Left) ? "1" : "3";
                    break;
                case ManeuverBearing.TallonRoll:
                    result = (Direction == ManeuverDirection.Left) ? ":" : ";";
                    break;
                case ManeuverBearing.Stationary:
                    result = "5";
                    break;
                case ManeuverBearing.Reverse:
                    switch (Direction)
                    {
                        case ManeuverDirection.Left:
                            result = "J";
                            break;
                        case ManeuverDirection.Forward:
                            result = "K";
                            break;
                        case ManeuverDirection.Right:
                            result = "L";
                            break;
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        public Color GetColor()
        {
            Color result = Color.yellow;
            switch (ColorComplexity)
            {
                case ManeuverColor.None:
                    break;
                case ManeuverColor.Green:
                    result = Color.green;
                    break;
                case ManeuverColor.White:
                    result = Color.white;
                    break;
                case ManeuverColor.Red:
                    result = Color.red;
                    break;
                default:
                    break;
            }
            return result;
        }

    }

}

