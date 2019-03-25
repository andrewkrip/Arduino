// Andrew Krippner

#include <Wire.h>
#include "Adafruit_MPRLS.h"
Adafruit_MPRLS mpr = Adafruit_MPRLS(-1, -1);

void setup() {
  beginSerialConnection();
  mpr.begin();
}

void loop() {
  if(Serial.available()) {
    delay(20);
    char head = Serial.read();
    switch (head) {
      case 'C': { //Request to connect
        Serial.println("Connect!");
        break;
      }
      case 'A': { //Request for analog value
        int pin = Serial.parseInt();
        pinMode(pin, INPUT);
        int value = analogRead(pin);
        Serial.println(value);
        break;
      }
      case 'D': { //Request for digital value
        int pin = Serial.parseInt();
        pinMode(pin, INPUT);
        int value = digitalRead(pin);
        Serial.println(value);
        break;
      }
      case 'W': { //Request to write digital value
        int pin = Serial.parseInt();
        int value = Serial.parseInt();
        pinMode(pin, OUTPUT);
        digitalWrite(pin, value);
        Serial.println("D");
        break;
      }
      case 'P': { //Request to write PWM value
        int pin = Serial.parseInt();
        int value = Serial.parseInt();
        pinMode(pin, OUTPUT);
        analogWrite(pin, value);
        Serial.println("D");
        break;
      }
      case 'S': { //Request for Special Value
        char key = Serial.read();
        if (key == 'P'){
//          float pressurePSI = mpr.readPressure() / 68.947572932 - 14.7;
//          Serial.println(pressurePSI);
          Serial.println(15.7);
        } else
          Serial.println("INVALID REQUEST");
        break;
      }
      default: { //Invalid request
        Serial.println("INVALID REQUEST");
      }
    }
    clearSerialBuffer();
    Serial.flush();
  }
}

void beginSerialConnection() {
  Serial.begin(9600);
  while (!Serial) { ; }
  clearSerialBuffer();
}

void clearSerialBuffer() {
  while (Serial.available() > 0) {
    char t = Serial.read();
  }
}
