﻿using System;
using System.Collections.Generic;
using System.IO;
using Model;
namespace ViewModel
{
    class Constantes
    {
        private int m_ACC_LIM_RPM_PER_SEC;
        public int ACC_LIM_RPM_PER_SEC { get { return m_ACC_LIM_RPM_PER_SEC; } }
        private int m_VEL_LIM_RPM;
        public int VEL_LIM_RPM { get { return m_ACC_LIM_RPM_PER_SEC; } }

        private int m_MOVE_DISTANCE_CNTS;
        public int MOVE_DISTANCE_CNTS { get { return m_MOVE_DISTANCE_CNTS; } }
        private int m_TIME_TILL_TIMEOUT;
        public int TIME_TILL_TIMEOUT { get { return m_TIME_TILL_TIMEOUT; } }
        private double m_ACCELERATION_MODEL_VMA;
        public double ACCELERATION_MODEL_VMA { get { return m_ACCELERATION_MODEL_VMA; } }
        private double m_ACCELERATION_MODEL_SLOPE;
        public double ACCELERATION_MODEL_SLOPE { get { return m_ACCELERATION_MODEL_SLOPE; } }
        private double m_ACCELERATION_MODEL_TORQUE_SENSITIVITY;
        public double ACCELERATION_MODEL_TORQUE_SENSITIVITY { get { return m_ACCELERATION_MODEL_TORQUE_SENSITIVITY; } }
        private double m_ACCELERATION_MODEL_TOQUE_FLEXION_POINT;
        public double ACCELERATION_MODEL_TOQUE_FLEXION_POINT { get { return m_ACCELERATION_MODEL_TOQUE_FLEXION_POINT; } }
        private double m_DECELERATION_MODEL_VMAX;
        public double DECELERATION_MODEL_VMAX { get { return m_DECELERATION_MODEL_VMAX; } }
        private double m_DECELERATION_MODEL_WIDTH;
        public double DECELERATION_MODEL_WIDTH { get { return m_DECELERATION_MODEL_WIDTH; } }
        private double m_DECELERATION_MODEL_SHIFT;
        public double DECELERATION_MODEL_SHIFT { get { return m_DECELERATION_MODEL_SHIFT; } }
        private double m_DECELERATION_MODEL_SLOPE;
        public double DECELERATION_MODEL_SLOPE { get { return m_DECELERATION_MODEL_SLOPE; } }
        private double m_DECELERATION_MODEL_DAMPING_COEFFICIENT;
        public double DECELERATION_MODEL_DAMPING_COEFFICIENT { get { return m_DECELERATION_MODEL_DAMPING_COEFFICIENT; } }
        private double m_ACCELERATION_MODEL_VMAX;
        public double ACCERATION_MODEL_VMAX { get { return m_ACCELERATION_MODEL_VMAX; } }
        private double m_EGG_VELOCITY;
        public double EGG_VELOCITY { get { return m_EGG_VELOCITY; } }
        private double m_EGG_TORQUE_SENSITIVITY;
        public double EGG_TORQUE_SENSITIVITY { get { return m_EGG_TORQUE_SENSITIVITY; } }

        string cgfPath;

        public Constantes()
        {
            Dictionary<string, string> DataDictionary = new Dictionary<string, string>();
            try
            {
                string[] lines = File.ReadAllLines(GetDirectory("config.cfg"));

                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                    {
                        string[] parts = line.Split('=');

                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                        DataDictionary[key] = value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite lors de la lecture du fichier de configuration : " + ex.Message);
            }

            m_ACC_LIM_RPM_PER_SEC = int.Parse(DataDictionary["ACC_LIM_RPM_PER_SEC"]);
            m_VEL_LIM_RPM = int.Parse(DataDictionary["VEL_LIM_RPM"]);
            m_MOVE_DISTANCE_CNTS = int.Parse(DataDictionary["MOVE_DISTANCE_CNTS"]);
            m_ACCELERATION_MODEL_VMAX = double.Parse(DataDictionary["ACCELERATION_MODEL_VMAX"]);
            m_ACCELERATION_MODEL_SLOPE = double.Parse(DataDictionary["ACCELERATION_MODEL_SLOPE"]);
            m_ACCELERATION_MODEL_TORQUE_SENSITIVITY = double.Parse(DataDictionary["ACCELERATION_MODEL_TORQUE_SENSITIVITY"]);
            m_ACCELERATION_MODEL_TOQUE_FLEXION_POINT = double.Parse(DataDictionary["ACCELERATION_MODEL_TOQUE_FLEXION_POINT"]);
            m_DECELERATION_MODEL_VMAX = double.Parse(DataDictionary["DECELERATION_MODEL_VMAX"]);
            m_DECELERATION_MODEL_WIDTH = double.Parse(DataDictionary["DECELERATION_MODEL_WIDTH"]);
            m_DECELERATION_MODEL_SHIFT = double.Parse(DataDictionary["DECELERATION_MODEL_SHIFT"]);
            m_DECELERATION_MODEL_SLOPE = double.Parse(DataDictionary["DECELERATION_MODEL_SLOPE"]);
            m_DECELERATION_MODEL_DAMPING_COEFFICIENT = double.Parse(DataDictionary["DECELERATION_MODEL_DAMPING_COEFFICIENT"]);
            m_EGG_VELOCITY = double.Parse(DataDictionary["EGG_VELOCITY"]);
            m_EGG_TORQUE_SENSITIVITY = double.Parse(DataDictionary["EGG_TORQUE_SENSITIVITY"]);
        }
        string GetDirectory(string cfgFileName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directoryInfo = new DirectoryInfo(currentDirectory);

            for (int i = 0; i < 4; i++)
            {
                if (directoryInfo.Parent != null)
                {
                    directoryInfo = directoryInfo.Parent;
                }
                else
                {
                    break; // Si le répertoire parent n'existe pas, on sort de la boucle
                }
            }

            return directoryInfo.FullName + "\\" + cfgFileName;
        }
    }
}
