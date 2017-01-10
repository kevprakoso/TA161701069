#!/usr/bin/python
import web
import smbus
import math
import pika
import json

import time

urls = (
    '/', 'index'
)

# Power management registers
power_mgmt_1 = 0x6b
power_mgmt_2 = 0x6c

bus = smbus.SMBus(1) # or bus = smbus.SMBus(1) for Revision 2 boards
address = 0x68       # This is the address value read via the i2cdetect command


def read_byte(adr):
    return bus.read_byte_data(address, adr)

def read_word(adr):
    high = bus.read_byte_data(address, adr)
    low = bus.read_byte_data(address, adr+1)
    val = (high << 8) + low
    return val

def read_word_2c(adr):
    val = read_word(adr)
    if (val >= 0x8000):
        return -((65535 - val) + 1)
    else:
        return val

def dist(a,b):
    return math.sqrt((a*a)+(b*b))

def get_y_rotation(x,y,z):
    radians = math.atan2(x, dist(y,z))
    return -math.degrees(radians)

def get_x_rotation(x,y,z):
    radians = math.atan2(y, dist(x,z))
    return math.degrees(radians)


def getSensorData():
    accel_xout = read_word_2c(0x3b)
    accel_yout = read_word_2c(0x3d)
    accel_zout = read_word_2c(0x3f)

    accel_xout_scaled = accel_xout / 16384.0
    accel_yout_scaled = accel_yout / 16384.0
    accel_zout_scaled = accel_zout / 16384.0
    return (str(accel_xout_scaled),str(accel_yout_scaled),str(accel_zout_scaled))

def send(message):
    #url = 'amqp://dgnutgjt:rejZ8AVH9lu5h7VDHtlWzttIRvcebbbt@spotted-monkey.rmq.cloudamqp.com/dgnutgjt'
    #params = pika.URLParameters(url)
    #params.socket_timeout = 10

    with open('config1.json') as configf:
        config = json.load(configf)
        
    url1 = '169.254.167.155'
    url2 = '192.168.1.130'
    url3 = '192.168.1.23'
    credential = pika.PlainCredentials(config['user'], config['password'])    
    connection = pika.BlockingConnection(pika.ConnectionParameters(
            host = config['host'], credentials = credential))
    
    channel = connection.channel()
    #channel.exchange_declare(exchange = 'data',type = 'fanout')
    #result = channel.queue_declare(queue='hello')
    channel.basic_publish(exchange='amq.topic', routing_key='emergency', body= message )
    connection.close();

while True:
    a_x, a_y, a_z  = getSensorData()
    lon = 107.610262
    lat = -6.892950
    msg = {"pointTime": "xxx", "timeZone": "Asia/Jakarta",
           "geometry": {
                   "type": "Feature",
                   "geometry": {
                       "type": "Point",
                       "coordinates": [lon, lat],
                    }},
           "accelerations": [
               {"x": float(a_x), "y": float(a_y), "z": float(a_z)}
            ] }
               
    text = '>{0:.5f}<{1:.5f}>{2:.5f}'.format(float(a_x), float(a_y),float(a_z))
    msgjson = json.dumps(msg) # buat dikirim
    
    send(msgjson)
    time.sleep(0.1)

    
    
