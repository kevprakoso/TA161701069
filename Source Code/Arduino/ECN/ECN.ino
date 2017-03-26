/*
   Earthquake Catcher Network v1.0
   Capstone Design by
    Christoporus Deo Putratama
    Kevin Shidqi
    Bramantio Yuwono

   Read seismic waves using IMU sensor
   Read location and exact time using GPS
   Sending those data using Wi-Fi using MQTT

*/


//====================================================//
//=================Library & Constant=================//
//====================================================//

#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include <SoftwareSerial.h>
#include <Wire.h>
#include <TinyGPS++.h>
#include <ArduinoJson.h>

#define SDA_PIN D2
#define SCL_PIN D1
#define PWR_MGMT 0x6B
#define RXPin D3
#define TXPin D4
#define GPSBaud 9600
#define NData 40 // Amount of Data per one second

const char* TIMEZONE = "Asia/Jakarta";
const char* PROP = "ITB";
const char* TYPE = "Point";
const int SendPeriod = 1000; //in ms
const float IMU_RES = 1.7;


//====================================================//
//==========Connection & Database Variables===========//
//====================================================//

const char* ssid = "LSKK Basement";    //  network SSID (name)
const char* pass = "noiznocon";   // network password
const char* mqtt_server = "black-boar.rmq.cloudamqp.com"; //MQTT server
const char* server_topic = "amq.topic.ecn"; //MQTT server topic
String mqtt_clientID = "ECN-1";
String mqtt_user = "lsowqccg:lsowqccg";
String mqtt_password = "kbLv9YbzjQwxz20NH7Rfy98TTV2eK17j";
int status = WL_IDLE_STATUS;
WiFiClient espClient;
PubSubClient client(espClient);

//====================================================//
//==================IMU & GPS Initiation==============//
//====================================================//

// I2C address of the MPU-9255
const int MPU = 0x68;

struct MPU9255 {
  float x;
  float y;
  float z;
};

MPU9255 data;

//TinyGPS++ Object
TinyGPSPlus gps;

// The serial connection to the GPS device
SoftwareSerial ss(RXPin, TXPin);

static const double init_lat = -6.889916, init_lon = 107.61133;
//===================================================//
//===================JSON OBJECT=====================//
//===================================================//
struct DataIMU {
  String x;
  String y;
  String z;
};

struct MessageData {
  String PointTime;
  String coordinates[2];
  DataIMU acc[NData];
};

struct SensorSetting {
  String ClientID;
  String TimeZone;
  String Interval;
  String Properties;
};

struct MessageData payload_data;
struct SensorSetting payload_setting;

int i = 0, j = 0;
bool checkgps = false;

//====================================================//
//===================MAIN ALGORITHM===================//
//====================================================//


void setup()
{
  MPU9255_Init();
  payload_setting = InitJsonObject(payload_setting);
  WiFiConnect();
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);

  Serial.begin(115200);
  Serial.println();
  ss.begin(GPSBaud);

  while (!checkgps)
  {
    while (ss.available() > 0)
    {
      if (gps.encode(ss.read()))
      {
        displayInfo();
        checkgps = true;
      }
    }
    if (millis() > 5000 && gps.charsProcessed() < 10)
    {
      payload_data.coordinates[0] = String(init_lat);
      payload_data.coordinates[1] = String(init_lon);
      checkgps = true;
    }
  }
  
}


void loop()
{
  if(!client.loop()) client.connect(mqtt_clientID.c_str(), mqtt_user.c_str(), mqtt_password.c_str());
  
  if (!client.connected()) {
    reconnect_server();
    i = 0;
  }
  else
  {
    data = Acc_Read();
    payload_data.acc[i].x = String(data.x,4);
    payload_data.acc[i].y = String(data.y,4);
    payload_data.acc[i].z = String(data.z,4);
    i++;
    
    if(i==NData)
    {
      i = 0;
      String YEAR = String(gps.date.year());
      String MONTH = String(gps.date.month());
      String DATE = String(gps.date.day());
      String HOUR = String(gps.time.hour());
      String MINUTE = String(gps.time.minute());
      String SECOND = String(gps.time.second());
      payload_data.PointTime = YEAR + "-" + MONTH + "-" + DATE + "T" + HOUR + ":" + MINUTE + ":" + SECOND + "Z";
      
      String message = JsonToString(payload_data,payload_setting);
      char message_t[2048];
      message.toCharArray(message_t, 2048);
      
      bool test = client.publish(server_topic, message_t);
      if(test)
        Serial.println("publish success");
    }
    delay(SendPeriod/NData);
  }
  yield();
}

//====================================================//
//==================ENCODE JSON FUNCTION==============//
//====================================================//


struct SensorSetting InitJsonObject(struct SensorSetting msg)
{
  msg.ClientID = mqtt_clientID;
  msg.TimeZone = TIMEZONE;
  msg.Interval = String(SendPeriod);
  msg.Properties = PROP;
  return msg;
}


String JsonToString(struct MessageData msg, struct SensorSetting set)
{
  String a = "";

  a = a + "{" + " \"pointTime\": " +  "\"" + msg.PointTime + "\"" + ",";
  a = a + "\"timeZone\":" + "\"" + set.TimeZone + "\"" + ",";
  a = a + "\"interval\":" + set.Interval  + ",";
  a = a + "\"clientID\":" + set.ClientID  + ",";
  a = a + "\"geojson\" :" + "{";

  //a = a + "\"type\":" + "\"" + msg.geometry.type + "\"" + ",";
  a = a + "\"geometry\":" + "{";

  a = a + "\"type\":" + "\"" + TYPE + "\"" + ",";
  a = a + "\"coordinates\": [ " + msg.coordinates[0] + ",";
  a = a + msg.coordinates[1] + "]";
  a = a + "},";

  a = a + "\"properties\":" + "{";
  a = a + "\"name\":" + "\"" + set.Properties + "\"";
  a = a + "}";

  a = a + "},";

  a = a + "\"accelerations\": [";

  for (int i = 0; i < NData; i++)
  {
    if (i != (NData-1))
    {
      a = a + "{";
      a = a + "\"x\": " + msg.acc[i].x  + ",";
      a = a + "\"y\": " + msg.acc[i].y  + ",";
      a = a + "\"z\": " + msg.acc[i].z  ;
      a = a + "},";
    }
    else
    {
      a = a + "{";
      a = a + "\"x\": " + msg.acc[i].x  + ",";
      a = a + "\"y\": " + msg.acc[i].y  + ",";
      a = a + "\"z\": " + msg.acc[i].z  ;
      a = a + "}";
    }

  }


  a = a + "]";

  a = a + "}";


  return a;
}


//====================================================//
//======Wi-Fi Connection & MQTT Function Procedure====//
//====================================================//
void WiFiConnect()
{
  // We start by connecting to a WiFi network
  WiFi.begin(ssid, pass);
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
  }
  randomSeed(micros());
}

void reconnect_server() {
  // Loop until we're reconnected
  while (!client.connected())
  {
    Serial.print("Attempting MQTT connection...");
    // Attempt to connect
    //if you MQTT broker has clientID,username and password
    //please change following line to    if (client.connect(clientId,userName,passWord))
    if (client.connect(mqtt_clientID.c_str(), mqtt_user.c_str(), mqtt_password.c_str()))
    {
      Serial.println("connected");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.print(" try again in" );
      Serial.print(String(SendPeriod));
      Serial.println("5 seconds");
      // Wait 1 Period before retrying
      delay(SendPeriod);
    }
  }
} //end reconnect()

void callback(char* topic, byte* payload, unsigned int length)
{

  
}

//====================================================//
//==============IMU & GPS Function Procedure==========//
//====================================================//

void MPU9255_Init()
{
  Wire.begin(SDA_PIN, SCL_PIN); // sda, scl
  Wire.beginTransmission(MPU);
  Wire.write(PWR_MGMT);  // PWR_MGMT_1 register
  Wire.write(0);     // set to zero (wakes up the MPU-6050)
  Wire.endTransmission(true);

}

struct MPU9255 Acc_Read()
{
  struct MPU9255 data;

  Wire.beginTransmission(MPU);
  Wire.write(0x3B);  // starting with register 0x3B (ACCEL_XOUT_H)
  Wire.endTransmission(false);
  Wire.requestFrom(MPU, 8, true); // request a total of 6 registers
  data.x = (float)(Wire.read() << 8 | Wire.read()) * IMU_RES; // 0x3B (ACCEL_XOUT_H) & 0x3C (ACCEL_XOUT_L)
  data.y = (float)(Wire.read() << 8 | Wire.read()) * IMU_RES; // 0x3D (ACCEL_YOUT_H) & 0x3E (ACCEL_YOUT_L)
  data.z = (float)(Wire.read() << 8 | Wire.read()) * IMU_RES; // 0x3F (ACCEL_ZOUT_H) & 0x40 (ACCEL_ZOUT_L)

  return data;
}

void displayInfo()
{
  if (gps.location.isValid())
  {
    payload_data.coordinates[0] = String (gps.location.lat(), 6);
    payload_data.coordinates[1] = String (gps.location.lng(), 6);
  }
  else
  {
    
    payload_data.coordinates[0] = String(init_lat);
    payload_data.coordinates[1] = String(init_lon);
  }
}
