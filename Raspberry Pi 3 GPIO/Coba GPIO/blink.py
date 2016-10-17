import RPi.GPIO as GPIO
import time

def main():
	GPIO.cleanup()
	GPIO.setmode(GPIO.BOARD) # to use Raspberry Pi board pin numbers
	GPIO.setup(11, GPIO.OUT) # set up GPIO output channel

	while True:
		GPIO.output(11, GPIO.LOW) # set RPi board pin 11 low. Turn off LED.
		time.sleep(1)
		GPIO.output(11, GPIO.HIGH) # set RPi board pin 11 high. Turn on LED.
		time.sleep(2)

main()