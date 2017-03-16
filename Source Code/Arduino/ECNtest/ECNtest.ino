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
#define IMU_RES 1
#define RXPin D3
#define TXPin D4
#define GPSBaud 9600
#define SendPeriod 1000 //in ms

const char* TIMEZONE = "Asia/Jakarta";



//====================================================//
//==========Connection & Database Variables===========//
//====================================================//

const char* ssid = "LSKK Basement";    //  network SSID (name)
const char* pass = "noiznocon";   // network password
const char* mqtt_server = "black-boar.rmq.cloudamqp.com"; //MQTT server
const char* server_topic = "amq.topic.ecn2"; //MQTT server topic
String mqtt_clientID = "ESP8266Client-1";
String mqtt_user = "lsowqccg:lsowqccg";
String mqtt_password = "kbLv9YbzjQwxz20NH7Rfy98TTV2eK17j";
int status = WL_IDLE_STATUS;
WiFiClient espClient;
PubSubClient client(espClient);
long lstMsg = 0;
int value = 0;




//====================================================//
//==================IMU & GPS Initiation==============//
//====================================================//

// I2C address of the MPU-6050
const int MPU = 0x68;

struct MPU6050 {
  uint8_t x;
  uint8_t y;
  uint8_t z;
};

MPU6050 data;

//TinyGPS++ Object
TinyGPSPlus gps;

// The serial connection to the GPS device
SoftwareSerial ss(RXPin, TXPin);

//===================================================//
//===================JSON OBJECT=====================//
//===================================================//
struct acc {
  String x;
  String y;
  String z;
};

struct geometry {
  String type;
  String coordinates[2];
};

struct prop {
  String Name;
};


struct geojson {
  String type;
  geometry geo;
  prop property;
};

struct data {
  String pointTime;
  String timeZone;
  String interval;
  geojson geometry;
  acc accelerations[20];
};

struct data msg;

struct data InitJsonObject(struct data msg);
String JsonToString(struct data msg);
int i = 0, j = 0;
bool checkgps = false;

//====================================================//
//===================MAIN ALGORITHM===================//
//====================================================//



//#include <TinyGPS++.h>
//#include <SoftwareSerial.h>
///*
//   This sample sketch demonstrates the normal use of a TinyGPS++ (TinyGPSPlus) object.
//   It requires the use of SoftwareSerial, and assumes that you have a
//   4800-baud serial GPS device hooked up on pins 4(rx) and 3(tx).
//*/
//static const int RXPin = D5, TXPin = D6;
//static const uint32_t GPSBaud = 19200;
//
//// The TinyGPS++ object
//TinyGPSPlus gps;
//
//// The serial connection to the GPS device
//SoftwareSerial ss(RXPin, TXPin);

void setup()
{

  // put your setup code here, to run once:
  pinMode(BUILTIN_LED, OUTPUT);     // Initialize the BUILTIN_LED pin as an output
  MPU6050_Init();
  //ss.begin(GPSBaud);
  //Serial.begin(9600);


  msg = InitJsonObject(msg);

  delay(100);
  WiFiConnect();
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);

  Serial.begin(115200);
  ss.begin(GPSBaud);

  Serial.println(F("DeviceExample.ino"));
  Serial.println(F("A simple demonstration of TinyGPS++ with an attached GPS module"));
  Serial.print(F("Testing TinyGPS++ library v. ")); Serial.println(TinyGPSPlus::libraryVersion());
  Serial.println(F("by Mikal Hart"));
  Serial.println();

  while (!checkgps)
  {
    while (ss.available() > 0)
      if (gps.encode(ss.read()))
      {
        displayInfo();
        checkgps = true;
      }
    if (millis() > 5000 && gps.charsProcessed() < 10)
    {
      Serial.println(F("No GPS detected: check wiring."));
      while (true);
    }
  }

}

void loop()
{


  // This sketch displays information every time a new sentence is correctly encoded.


  data = Acc_Read();
  msg.accelerations[i].x = data.x;
  msg.accelerations[i].y = data.y;
  msg.accelerations[i].z = data.z;

  i++;
  if (!client.connected()) {
    reconnect_server();
  }
  client.loop();
  long now = millis();
  //if (now - lstMsg > SendPeriod)
  if (i == 20)
  {
    i = 0;
    String YEAR = String(gps.date.year());
    String MONTH = String(gps.date.month());
    String DATE = String(gps.date.day());
    String HOUR = String(gps.time.hour());
    String MINUTE = String(gps.time.minute());
    String SECOND = String(gps.time.second());
    msg.pointTime = YEAR + "-" + MONTH + "-" + DATE + "T" + HOUR + ":" + MINUTE + ":" + SECOND + "Z";

    //lstMsg = now;
    String message = JsonToString(msg);
    //msg.printTo(message);
    char message_t[800];
    message.toCharArray(message_t, 800);
    //publish sensor data to MQTT broker
    bool test = client.publish(server_topic, message_t);
    if (test)
      Serial.println("publish success");
  }
  delay(50);
}

void displayInfo()
{
  Serial.print(F("Location: "));
  if (gps.location.isValid())
  {
    Serial.print(gps.location.lat(), 6);
    msg.geometry.geo.coordinates[0] = String (gps.location.lat(), 6);
    Serial.print(F(","));
    Serial.print(gps.location.lng(), 6);
    msg.geometry.geo.coordinates[1] = String (gps.location.lng(), 6);
  }
  else
  {
    Serial.print(F("INVALID"));
  }

  Serial.print(F("  Date/Time: "));
  if (gps.date.isValid())
  {
    Serial.print(gps.date.month());
    Serial.print(F("/"));
    Serial.print(gps.date.day());
    Serial.print(F("/"));
    Serial.print(gps.date.year());
  }
  else
  {
    Serial.print(F("INVALID"));
  }

  Serial.print(F(" "));
  if (gps.time.isValid())
  {
    if (gps.time.hour() < 10) Serial.print(F("0"));
    Serial.print(gps.time.hour());
    Serial.print(F(":"));
    if (gps.time.minute() < 10) Serial.print(F("0"));
    Serial.print(gps.time.minute());
    Serial.print(F(":"));
    if (gps.time.second() < 10) Serial.print(F("0"));
    Serial.print(gps.time.second());
    Serial.print(F("."));
    if (gps.time.centisecond() < 10) Serial.print(F("0"));
    Serial.print(gps.time.centisecond());
  }
  else
  {
    Serial.print(F("INVALID"));
  }

  Serial.println();
}




//====================================================//
//======Wi-Fi Connection & MQTT Function Procedure====//
//====================================================//
void WiFiConnect()
{
  // We start by connecting to a WiFi network
  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, pass);
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }
  randomSeed(micros());
  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

void reconnect_server() {
  // Loop until we're reconnected
  while (!client.connected())
  {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);
    // Attempt to connect
    //if you MQTT broker has clientID,username and password
    //please change following line to    if (client.connect(clientId,userName,passWord))
    if (client.connect(mqtt_clientID.c_str(), mqtt_user.c_str(), mqtt_password.c_str()))
    {
      Serial.println("connected");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 6 seconds before retrying
      delay(6000);
    }
  }
} //end reconnect()

void callback(char* topic, byte* payload, unsigned int length)
{
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();

  // Switch on the LED if an 1 was received as first character
  if ((char)payload[0] == '1') {
    digitalWrite(BUILTIN_LED, LOW);   // Turn the LED on (Note that LOW is the voltage level
    // but actually the LED is on; this is because
    // it is acive low on the ESP-01)
  } else {
    digitalWrite(BUILTIN_LED, HIGH);  // Turn the LED off by making the voltage HIGH
  }

}


//====================================================//
//==============IMU & GPS Function Procedure==========//
//====================================================//

void MPU6050_Init()
{
  Wire.begin(SDA_PIN, SCL_PIN); // sda, scl
  Wire.beginTransmission(MPU);
  Wire.write(PWR_MGMT);  // PWR_MGMT_1 register
  Wire.write(0);     // set to zero (wakes up the MPU-6050)
  Wire.endTransmission(true);

}

MPU6050 Acc_Read()
{
  MPU6050 data;

  Wire.beginTransmission(MPU);
  Wire.write(0x3B);  // starting with register 0x3B (ACCEL_XOUT_H)
  Wire.endTransmission(false);
  Wire.requestFrom(MPU, 8, true); // request a total of 6 registers
  data.x = (Wire.read() << 8 | Wire.read()) * IMU_RES; // 0x3B (ACCEL_XOUT_H) & 0x3C (ACCEL_XOUT_L)
  data.y = (Wire.read() << 8 | Wire.read()) * IMU_RES; // 0x3D (ACCEL_YOUT_H) & 0x3E (ACCEL_YOUT_L)
  data.z = (Wire.read() << 8 | Wire.read()) * IMU_RES; // 0x3F (ACCEL_ZOUT_H) & 0x40 (ACCEL_ZOUT_L)

  return data;
}


//====================================================//
//==================ENCODE JSON FUNCTION==============//
//====================================================//


struct data InitJsonObject(struct data msg)
{
  static const double init_lat = -6.889916, init_lon = 107.61133;
  msg.pointTime = "test";
  msg.timeZone = "Asia/Jakarta";
  msg.interval = "500";
  msg.geometry.type = "Feature";
  msg.geometry.geo.type = "Point";
  msg.geometry.geo.coordinates[0] = String(init_lat);
  msg.geometry.geo.coordinates[1] = String(init_lon);
  msg.geometry.property.Name = "ITB";

  return msg;
}


String JsonToString(struct data msg)
{
  String a = "";

  a = a + "{" + " \"pointTime\": " +  "\"" + msg.pointTime + "\"" + ",";
  a = a + "\"timeZone\":" + "\"" + msg.timeZone + "\"" + ",";
  a = a + "\"interval\":" + msg.interval  + ",";
  a = a + "\"geojson\" :" + "{";

  a = a + "\"type\":" + "\"" + msg.geometry.type + "\"" + ",";
  a = a + "\"geometry\":" + "{";

  a = a + "\"type\":" + "\"" + msg.geometry.geo.type + "\"" + ",";
  a = a + "\"coordinates\": [ " + msg.geometry.geo.coordinates[0] + ",";
  a = a + msg.geometry.geo.coordinates[1] + "]";
  a = a + "},";

  a = a + "\"properties\":" + "{";
  a = a + "\"name\":" + "\"" + msg.geometry.property.Name + "\"";
  a = a + "}";

  a = a + "},";

  a = a + "\"accelerations\": [";

  for (int i = 0; i < 20; i++)
  {
    if (i != 19)
    {
      a = a + "{";
      a = a + "\"x\": " + msg.accelerations[i].x  + ",";
      a = a + "\"y\": " + msg.accelerations[i].y  + ",";
      a = a + "\"z\": " + msg.accelerations[i].z  ;
      a = a + "},";
    }
    else
    {
      a = a + "{";
      a = a + "\"x\": " + msg.accelerations[i].x  + ",";
      a = a + "\"y\": " + msg.accelerations[i].y  + ",";
      a = a + "\"z\": " + msg.accelerations[i].z  ;
      a = a + "}";
    }

  }


  a = a + "]";

  a = a + "}";


  return a;
}




