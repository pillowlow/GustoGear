
#include <BluetoothSerial.h>
#include <Adafruit_MPR121.h>
#include <Stepper.h>

// Bluetooth Serial object for ESP32
BluetoothSerial SerialBT;

// MPR121 setup
Adafruit_MPR121 cap = Adafruit_MPR121();
#define MPR121_ADDR 0x5A

// Stepper motor setup
const int stepsPerRevolution = 4096; // Adjust based on your stepper motor
Stepper stepper(stepsPerRevolution, 13, 12, 14, 27); // IN1, IN2, IN3, IN4

void setup() {
  Serial.begin(115200);
  SerialBT.begin("ESP32_BT"); // Bluetooth device name
  if (!cap.begin(MPR121_ADDR)) {
    Serial.println("MPR121 not found, check wiring?");
    while (1);
  }

  stepper.setSpeed(15); // Set stepper motor speed

  Serial.println("Setup complete.");
}

void loop() {
  static unsigned long lastTouchTime = 0;
  uint16_t touched = cap.touched();
  
  for (int i = 0; i < 12; i++) {
    if (touched & (1 << i)) {
      if (millis() - lastTouchTime > 500) { // Example for debouncing
        SerialBT.print("Pin "); SerialBT.print(i); SerialBT.println(" touched.");
        lastTouchTime = millis();
      }
    }
  }

  // Check for Bluetooth commands
  if (SerialBT.available()) {
    String command = SerialBT.readString();
    handleBluetoothCommand(command);
  }
}

void handleBluetoothCommand(String command) {
  // Parse command and convert to rotation angle or other actions
  // Example: if command is "Rotate 1", rotate by some calculated angle
  if (command.startsWith("Rotate ")) {
    int segment = command.substring(7).toInt(); // Get segment number
    int angle = (segment - 1) * (360 / 12); // Calculate angle for a dodecagon
    rotateStepperToAngle(angle);
  }
}

void rotateStepperToAngle(int angle) {
  int stepsToMove = map(angle, 0, 360, 0, stepsPerRevolution);
  stepper.step(stepsToMove);
}
