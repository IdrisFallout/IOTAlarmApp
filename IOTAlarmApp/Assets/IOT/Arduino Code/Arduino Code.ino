#include <ArduinoJson.h>
#include <virtuabotixRTC.h>
#include <Arduino.h>
#include "DynamicArray.h"

#define buzzerPin 3

virtuabotixRTC myRTC(6, 7, 8);

int theHour = 0;
int theMinutes = 0;
int theSeconds = 0;

String theTime = "";

int secondToggle = 0;
int secondBuffer[] = { 0, 0 };

int hours_counter = 0;
int hours_tracker[2];

int minutes_counter = 0;
int minutes_tracker[2];

bool isRinging = false;

String AlarmString = "";
String currentTime = "";

unsigned long previousMillis = 0;  // Store the previous time in milliseconds
unsigned long interval = 100;

#define INTERVAL_ON 100
#define INTERVAL_OFF_SHORT 100
#define INTERVAL_OFF_LONG 700
int beepState = 0;

DynamicArray<String> nodes;

void addNode(const String& time) {
  nodes.push_back(time);  // Add new node to the DynamicArray
}

void printNodes() {
  for (int i = 0; i < nodes.size(); i++) {
    Serial.print("Alarm ");
    Serial.print(i + 1);
    Serial.print(": ");
    String theActualAlarm = convertToMilitaryTime(nodes[i]);
    Serial.println(theActualAlarm);
    if (theActualAlarm == formatTime(currentTime)) {
      isRinging = true;
    }
  }
}

void clearNodes() {
  nodes.clear();  // Clear all nodes in the DynamicArray
}

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(buzzerPin, OUTPUT);
  digitalWrite(buzzerPin, LOW);
  // seconds, minutes, hours, day of the week, day of the month, month, year
  // myRTC.setDS1302Time(0, 49, 1, 6, 15, 12, 2023);
}

void loop() {
  ReadRTC();
  parse_time(theTime);
  if (Serial.available()) {                   // Check if data is available to read
    String s = Serial.readStringUntil('\n');  // Read the incoming string until newline character
    // Serial.println(s);

    if (s.startsWith("[")) {
      beep(100);
      isRinging = false;
      digitalWrite(buzzerPin, LOW);
      if (s.charAt(0) == '[' && s.charAt(1) == ']') {
        clearNodes();
      } else {
        parse_alarms(s);
      }
    }
  }

  ringing_sequence();
}

void parse_time(String s) {
  int firstColonIndex = s.indexOf(':');                        // Find the index of the first colon
  int secondColonIndex = s.indexOf(':', firstColonIndex + 1);  // Find the index of the second colon

  if (firstColonIndex != -1 && secondColonIndex != -1) {  // Check if both colons were found
    String hourString = s.substring(0, firstColonIndex);  // Extract the minutes string
    String minuteString = s.substring(firstColonIndex + 1, secondColonIndex);
    currentTime = hourString + ":" + minuteString;
    int hours = hourString.toInt();  // Convert the minutes string to an integer
    int minutes = minuteString.toInt();

    if (minutes_counter == 0) {
      minutes_tracker[0] = minutes;
      minutes_counter = 1;
    } else if (minutes_counter == 1) {
      minutes_tracker[1] = minutes;
      minutes_counter = 0;
    }

    if (hours_counter == 0) {
      hours_tracker[0] = hours;
      hours_counter = 1;
    } else if (hours_counter == 1) {
      hours_tracker[1] = hours;
      hours_counter = 0;
    }

    if (hours_tracker[0] == hours_tracker[1]) {

    } else {
      beep(100);
    }

    if (minutes_tracker[0] == minutes_tracker[1]) {

    } else {
      StartAlarm();
    }
  }
}

void ReadRTC() {
  myRTC.updateTime();

  theHour = myRTC.hours;
  theMinutes = myRTC.minutes;
  theSeconds = myRTC.seconds;

  if (secondToggle == 0) {
    secondBuffer[0] = theSeconds;
    secondToggle = 1;
  } else if (secondToggle == 1) {
    secondBuffer[1] = theSeconds;
    secondToggle = 0;
  }

  if (secondBuffer[0] == secondBuffer[1]) return;

  theTime = (String)myRTC.hours + ":" + (String)myRTC.minutes + ":" + (String)myRTC.seconds;
  Serial.println(theTime);
}

void parse_alarms(String s) {
  clearNodes();
  storeTimeValues(s);
  StartAlarm();
}

String convertToMilitaryTime(String timeStr) {
  int hour = timeStr.substring(0, 2).toInt();
  int minute = timeStr.substring(3, 5).toInt();
  String amPm = timeStr.substring(6);

  // Convert to military time
  if (amPm == "PM" && hour != 12) {
    hour += 12;
  } else if (amPm == "AM" && hour == 12) {
    hour = 0;
  }

  // Format the result as a string in military time
  String militaryTime = String(hour < 10 ? "0" : "") + String(hour) + ":" + String(minute < 10 ? "0" : "") + String(minute);
  return militaryTime;
}

void StartAlarm() {
  Serial.println("Check Alarms....");
  isRinging = false;
  digitalWrite(buzzerPin, LOW);
  printNodes();
}

void ringing_sequence() {
  if (isRinging == true) {
    unsigned long currentMillis = millis();  // Get the current time in milliseconds

    // Check the current state of the beep sequence
    if (beepState == 0 && currentMillis - previousMillis >= INTERVAL_ON) {
      digitalWrite(buzzerPin, HIGH);   // Turn on the buzzer
      previousMillis = currentMillis;  // Update the previous time
      beepState = 1;                   // Move to the next state
    } else if (beepState == 1 && currentMillis - previousMillis >= INTERVAL_OFF_SHORT) {
      digitalWrite(buzzerPin, LOW);    // Turn off the buzzer
      previousMillis = currentMillis;  // Update the previous time
      beepState = 2;                   // Move to the next state
    } else if (beepState == 2 && currentMillis - previousMillis >= INTERVAL_ON) {
      digitalWrite(buzzerPin, HIGH);   // Turn on the buzzer
      previousMillis = currentMillis;  // Update the previous time
      beepState = 3;                   // Move to the next state
    } else if (beepState == 3 && currentMillis - previousMillis >= INTERVAL_OFF_SHORT) {
      digitalWrite(buzzerPin, LOW);    // Turn off the buzzer
      previousMillis = currentMillis;  // Update the previous time
      beepState = 4;                   // Move to the next state
    } else if (beepState == 4 && currentMillis - previousMillis >= INTERVAL_OFF_LONG) {
      previousMillis = currentMillis;  // Update the previous time
      beepState = 0;                   // Reset the beep sequence
    }
  }
}


void beep(int the_delay) {
  digitalWrite(buzzerPin, HIGH);
  delay(the_delay);
  digitalWrite(buzzerPin, LOW);
}

void storeTimeValues(const String& input) {
  // Remove square brackets from the input string
  String inputWithoutBrackets = input.substring(1, input.length() - 1);

  // Split the input string into individual time values based on commas
  int startPos = 0;
  int endPos = 0;
  while (endPos >= 0) {
    endPos = inputWithoutBrackets.indexOf(',', startPos);
    if (endPos >= 0) {
      String timeValue = inputWithoutBrackets.substring(startPos + 1, endPos - 1);  // Skip the square bracket, space, and quotation mark after the comma
      addNode(timeValue);
      startPos = endPos + 2;  // Skip the comma and space after it
    } else {
      String timeValue = inputWithoutBrackets.substring(startPos + 1, inputWithoutBrackets.length() - 2);  // Skip the square bracket, space, and quotation mark
      addNode(timeValue);
    }
  }
}

String formatTime(String timeString) {
  // Extract hours and minutes from input string
  int hours = timeString.substring(0, timeString.indexOf(':')).toInt();
  int minutes = timeString.substring(timeString.indexOf(':') + 1).toInt();

  // Format hours with leading zero if necessary
  String formattedHours = (hours < 10) ? "0" + String(hours) : String(hours);

  // Format minutes with leading zero if necessary
  String formattedMinutes = (minutes < 10) ? "0" + String(minutes) : String(minutes);

  // Return formatted time string
  return formattedHours + ":" + formattedMinutes;
}