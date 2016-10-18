#include <ESP8266WiFi.h>
#include <Wire.h>

#define myPeriodic 15 //in sec | Thingspeak pub is 15sec

const char* server = "api.thingspeak.com";
String apiKey ="2QEZILOPGTRE0K5Y";
const char* MY_SSID = "marb"; 
const char* MY_PWD = "123456!a";
int sent = 0;

const int MPU=0x68;  // I2C address of the MPU-6050

struct MPU6050{
  int16_t AcX;
  int16_t AcY;
  int16_t AcZ;
} ;

MPU6050 data;

void MPU6050_Init();
void connectWiFi();
MPU6050 Acc_Read();
void Send_Acc(MPU6050 data);

void setup() {
  // put your setup code here, to run once
  Serial.begin(115200);
  connectWifi();
  MPU6050_Init();
}

void loop() {
  // put your main code here, to run repeatedly:
  data = Acc_Read();
  Send_Acc(data);
}


void MPU6050_Init()
{
  Wire.begin(4, 5); // sda, scl
  Wire.beginTransmission(MPU);
  Wire.write(0x6B);  // PWR_MGMT_1 register
  Wire.write(0);     // set to zero (wakes up the MPU-6050)
  Wire.endTransmission(true);
  Serial.println("\nSetup complete...");  
  Serial.println("  Acc X\t  Acc Y\t  Acc Z\n");
}


MPU6050 Acc_Read()
{
  MPU6050 data;
  
  Wire.beginTransmission(MPU);
  Wire.write(0x3B);  // starting with register 0x3B (ACCEL_XOUT_H)
  Wire.endTransmission(false);
  Wire.requestFrom(MPU,6,true);  // request a total of 6 registers
  data.AcX=Wire.read()<<8|Wire.read();  // 0x3B (ACCEL_XOUT_H) & 0x3C (ACCEL_XOUT_L)    
  data.AcY=Wire.read()<<8|Wire.read();  // 0x3D (ACCEL_YOUT_H) & 0x3E (ACCEL_YOUT_L)
  data.AcZ=Wire.read()<<8|Wire.read();  // 0x3F (ACCEL_ZOUT_H) & 0x40 (ACCEL_ZOUT_L)
  Serial.print("  " + String(data.AcX) + "\t");
  Serial.print("  " + String(data.AcY) + "\t");
  Serial.println("  " + String(data.AcZ)+ "\t");  

  delay(333);
  return data;
}

void connectWifi()
{
  Serial.print("\nConnecting to "+String(MY_SSID) + "\n");
  WiFi.begin(MY_SSID, MY_PWD);
  while (WiFi.status() != WL_CONNECTED) {
  delay(100);
  Serial.print(".");
  }  
  Serial.println("\n");
  Serial.println("Connected");
  Serial.println("");  
}//end connect

void Send_Acc(MPU6050 data)
{
     WiFiClient client;
  
   if (client.connect(server, 80)) { // use ip 184.106.153.149 or api.thingspeak.com
   Serial.println("WiFi Client connected ");
   
   String postStr = apiKey;
   //Entry data Acc X to field 1 
   postStr += "&field1=";
   postStr += String(data.AcX);
   
   //Entry data Acc Y to field 2
   postStr += "&field2=";
   postStr += String(data.AcY);
   
   //Entry data Acc Z to field 3
   postStr += "&field3=";
   postStr += String(data.AcZ);
   postStr += "\r\n\r\n";
   
   client.print("POST /update HTTP/1.1\n");
   client.print("Host: api.thingspeak.com\n");
   client.print("Connection: close\n");
   client.print("X-THINGSPEAKAPIKEY: " + apiKey + "\n");
   client.print("Content-Type: application/x-www-form-urlencoded\n");
   client.print("Content-Length: ");
   client.print(postStr.length());
   client.print("\n\n");
   client.print(postStr);
   
   delay(1000);
   
   }//end if
   sent++;
 client.stop();
}
