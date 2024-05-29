#include <Stepper.h>
#include <BluetoothSerial.h>
#include <Adafruit_MPR121.h>

BluetoothSerial SerialBT;

const int stepsPerRevolution = 2048; // Total steps per full revolution for a 28BYJ-48 stepper motor
const int motorSpeed = 12; // RPM for the stepper motor
Stepper myStepper(stepsPerRevolution, 13, 12, 14, 27);

// MPR121 setup
Adafruit_MPR121 cap = Adafruit_MPR121();
#define MPR121_ADDR 0x5A

int currentAngle = 0; // This will keep track of the current angle position of the stepper

void setup() {
  Serial.begin(9600);
  SerialBT.begin("ESP32_BT");
  myStepper.setSpeed(motorSpeed);
  
  if (!cap.begin(MPR121_ADDR)) {
    Serial.println("MPR121 not found, check wiring?");
    while (1);
  }
  Serial.println("Setup complete.");

}

void loop() {
  if (SerialBT.available()) {
    String command = SerialBT.readString();
    command.trim();
    if (command.length() > 0) { // Check if command is not empty
      int targetAngle = command.toInt();
      rotateStepperToAngle(targetAngle);
    }
  }
  // mpr121
  static unsigned long lastTouchTime = 0;
  uint16_t touched = cap.touched();
  
  for (int i = 0; i < 12; i++) {
    if (touched & (1 << i)) {
      if (millis() - lastTouchTime > 500) {  // Debouncing
        SerialBT.print("Pin "); SerialBT.print(i); SerialBT.println(" touched.");
        lastTouchTime = millis();
      }
    }
  }

  
}

void rotateStepperToAngle(int targetAngle) {
  int angleDifference = targetAngle - currentAngle;
  
  // Normalize the angle difference to be within -180 to 180 degrees
  if (angleDifference > 180) {
    angleDifference -= 360;
  } else if (angleDifference < -180) {
    angleDifference += 360;
  }

  int stepsToMove = map(angleDifference, -180, 180, -stepsPerRevolution / 2, stepsPerRevolution / 2);
  myStepper.step(stepsToMove);

  currentAngle = targetAngle; // Update the current angle
  currentAngle %= 360; // Ensure the angle stays within 0-359 degrees
  if (currentAngle < 0) currentAngle += 360; // Correct for negative wrap-around

  SerialBT.println("Moved to angle: " + String(targetAngle));
}
