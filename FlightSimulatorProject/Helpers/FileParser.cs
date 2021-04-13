using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using System.Globalization;




namespace FlightSimulatorProject.Helpers
{
    public class FileParser { 

        public Dictionary<string, List<float>> DataDictC = new Dictionary<string, List<float>>();

        public Dictionary<long, List<float>> DataDictR = new Dictionary<long, List<float>>();

        public Dictionary<string, List<double>> DataDict = new Dictionary<string, List<double>>();


        public List<string> optionList = new List<string>
            {"aileron","elevator","rudder","flaps","slats","speedbreak","throttle0","throttle1",
            "engine-pump0","engine-pump1","electric-pump0","electric-pump1","external-power","APU-generator",
            "latitude-deg","longitude-deg","altitude-ft","roll-deg","pitch-deg","heading-deg",
            "side-slip-deg","airspeed-kt","glideslope","vertical-speed-fps","airspeed-indicator_indicated-speed-kt",
            "altimeter_indicated-altitude-ft","altimeter_pressure-alt-ft",
            "attitude-indicator_indicated-pitch-deg","attitude-indicator_indicated-roll-deg",
            "attitude-indicator_internal-pitch-deg","attitude-indicator_internal-roll-deg",
            "encoder_indicated-altitude-ft","encoder_pressure-alt-ft","gps_indicated-altitude-ft",
            "gps_indicated-ground-speed-kt","gps_indicated-vertical-speed",
            "indicated-heading-deg","magnetic-compass_indicated-heading-deg",
            "slip-skid-ball_indicated-slip-skid","turn-indicator_indicated-turn-rate"
            ,"vertical-speed-indicator_indicated-speed-fpm","engine_rpm"};


    public void initializeData()
    {
        string filePath = System.IO.Path.GetFullPath("reg_flight.csv");

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {

            foreach (string s in optionList)
            {
                DataDict.Add(s, new List<double>());
                DataDictC.Add(s, new List<float>());
            }

            long counter = 1;
            while (csv.Read())

            {
                List<float> lineData = new List<float>();

                int j = 0;
                foreach (string s in optionList)
                {
                    lineData.Add(float.Parse(csv.GetField(j)));
                    j++;
                }

                DataDictR.Add(counter, lineData);
                int i = 0;
                foreach (string s in optionList)
                {
                    DataDict[s].Add(Convert.ToDouble(csv.GetField(i)));
                    DataDictC[s].Add(float.Parse(csv.GetField(i)));
                    i++;
                }
                counter++;
            }

        }

    }
}
}
        /*
        {
        
        public Dictionary<string, List<double>> DataDict = new Dictionary<string, List<double>>();

        public Dictionary<string, List<float>> DataDictC = new Dictionary<string, List<float>>();
        public Dictionary<long, List<float>> DataDictR = new Dictionary<long, List<float>>();

        public List<string> optionList = new List<string>
            {"aileron","elevator","rudder","flaps","slats","speedbreak","throttle0","throttle1",
            "engine-pump0","engine-pump1","electric-pump0","electric-pump1","external-power","APU-generator",
            "latitude-deg","longitude-deg","altitude-ft","roll-deg","pitch-deg","heading-deg",
            "side-slip-deg","airspeed-kt","glideslope","vertical-speed-fps","airspeed-indicator_indicated-speed-kt",
            "altimeter_indicated-altitude-ft","altimeter_pressure-alt-ft",
            "attitude-indicator_indicated-pitch-deg","attitude-indicator_indicated-roll-deg",
            "attitude-indicator_internal-pitch-deg","attitude-indicator_internal-roll-deg",
            "encoder_indicated-altitude-ft","encoder_pressure-alt-ft","gps_indicated-altitude-ft",
            "gps_indicated-ground-speed-kt","gps_indicated-vertical-speed",
            "indicated-heading-deg","magnetic-compass_indicated-heading-deg",
            "slip-skid-ball_indicated-slip-skid","turn-indicator_indicated-turn-rate"
            ,"vertical-speed-indicator_indicated-speed-fpm","engine_rpm"};


        public void initializeData()
        {
            string filePath = System.IO.Path.GetFullPath("reg_flight.csv");

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

                foreach (string s in optionList)
                {

                    DataDict.Add(s, new List<double>());
                }

                while (csv.Read())
                {
                    int i = 0;
                    foreach (string s in optionList)
                    {
                        DataDict[s].Add(Convert.ToDouble(csv.GetField(i)));
                        i++;
                    }

                }

            }
        }
    }
    }
*/