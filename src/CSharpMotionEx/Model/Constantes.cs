using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Model;
namespace ViewModel
{
    class Constantes
    {
        private bool m_USE_UNITY_WITH_MQTT;
        public bool USE_UNITY_WITH_MQTT { get { return m_USE_UNITY_WITH_MQTT; } }
        private int m_ACC_LIM_RPM_PER_SEC;
        public int ACC_LIM_RPM_PER_SEC { get { return m_ACC_LIM_RPM_PER_SEC; } }

        private int m_VEL_LIM_RPM;
        public int VEL_LIM_RPM { get { return m_VEL_LIM_RPM; } }

        private int m_MOVE_DISTANCE_CNTS;
        public int MOVE_DISTANCE_CNTS { get { return m_MOVE_DISTANCE_CNTS; } }

        private int m_TIME_TILL_TIMEOUT;
        public int TIME_TILL_TIMEOUT { get { return m_TIME_TILL_TIMEOUT; } }

        private int m_NUM_MOVES;
        public int NUM_MOVES { get { return m_NUM_MOVES; } }

        private double m_ACCELERATION_MODEL_VMAX;
        public double ACCELERATION_MODEL_VMAX { get { return m_ACCELERATION_MODEL_VMAX; } }

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

        private double m_EGG_VELOCITY;
        public double EGG_VELOCITY { get { return m_EGG_VELOCITY; } }

        private double m_EGG_TORQUE_SENSITIVITY;
        public double EGG_TORQUE_SENSITIVITY { get { return m_EGG_TORQUE_SENSITIVITY; } }

        private double m_MOTOR_SEUIL;
        public double MOTOR_SEUIL { get { return m_MOTOR_SEUIL; } }

        private Dictionary<string, string> m_GetConstantsList;
        public Dictionary<string, string> GetConstantsList { get { return m_GetConstantsList; } }

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
                m_GetConstantsList = DataDictionary;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite lors de la lecture du fichier de configuration : " + ex.Message);
            }
            m_USE_UNITY_WITH_MQTT = bool.Parse(DataDictionary["USE_UNITY_WITH_MQTT"]);
            m_ACC_LIM_RPM_PER_SEC = int.Parse(DataDictionary["ACC_LIM_RPM_PER_SEC"]);
            m_VEL_LIM_RPM = int.Parse(DataDictionary["VEL_LIM_RPM"]);
            m_MOVE_DISTANCE_CNTS = int.Parse(DataDictionary["MOVE_DISTANCE_CNTS"]);
            m_NUM_MOVES = int.Parse(DataDictionary["NUM_MOVES"]);
            m_TIME_TILL_TIMEOUT = int.Parse(DataDictionary["TIME_TILL_TIMEOUT"]);
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
            m_MOTOR_SEUIL = double.Parse(DataDictionary["MOTOR_SEUIL"]);
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

        public void WriteConstant(string key, string value)
        {
            try
            {
                string path = GetDirectory("config.cfg");

                if (!File.Exists(path))
                {
                    Console.WriteLine("File does not exist.");
                    return;
                }

                var lines = File.ReadAllLines(path).ToList();
                int index = lines.FindIndex(line => line.StartsWith(key + "="));

                // If key already exists, update its value
                if (index != -1)
                {
                    lines[index] = key + "=" + value;
                }
                // Otherwise, add a new line with key and value
                else
                {
                    lines.Add(key + "=" + value);
                }

                // Write the lines back to the file
                File.WriteAllLines(path, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
