#include <ArduinoJson.h>
#include <virtuabotixRTC.h>
#include <Arduino.h>
#include "DynamicArray.h"

// #######################################################

DynamicArray<String> nodes;

void addNode(const String& time) {
  nodes.push_back(time);  // Add new node to the DynamicArray
}

void printNodes() {
  Serial.println("Nodes:");
  for (int i = 0; i < nodes.size(); i++) {
    Serial.print("Node ");
    Serial.print(i + 1);
    Serial.print(": ");
    Serial.println(nodes[i]);
  }
}

void clearNodes() {
  nodes.clear();  // Clear all nodes in the DynamicArray
}
// ##########################################################

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

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(buzzerPin, OUTPUT);
  digitalWrite(buzzerPin, LOW);
}

void loop() {
  ReadRTC();
  parse_time(theTime);
  if (Serial.available()) {                   // Check if data is available to read
    String s = Serial.readStringUntil('\n');  // Read the incoming string until newline character
    Serial.println(s);

    if (s.startsWith("[")) {
      parse_alarms(s);
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
  // DynamicJsonDocument doc(1024);
  // DeserializationError error = deserializeJson(doc, s);
  // if (error) {
  //   Serial.print("deserializeJson() failed: ");
  //   Serial.println(error.c_str());
  //   return;
  // }
  beep(100);
  // JsonArray arr = doc.as<JsonArray>();
  clearNodes();
  storeTimeValues(s);
  // for (const auto& val : arr) {
  //   addNode(val.as<String>());
  //   Serial.println(val.as<String>());
  // }
  
  StartAlarm();
}

String extract_time_string(String input_string) {
  String time_string = "";

  // Find the position of the colon after "Now:"
  int colon_pos = input_string.indexOf(":");
  if (colon_pos != -1) {  // If colon is found
    // Extract the substring after the colon
    time_string = input_string.substring(colon_pos + 1);
  }

  return time_string;
}

String convertToMilitaryTime(String timeStr) {
  // Extract hour, minute, and AM/PM from the time string
  int hour = timeStr.substring(0, 2).toInt();
  int minute = timeStr.substring(3, 5).toInt();
  String amPm = timeStr.substring(6);
  Serial.println("HOUR : " + (String)hour);
  Serial.println("MINUTES : " + (String)minute);
  Serial.println("AM/PM : " + (String)amPm);

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


void readAndPrintStrings(String inputString) {
  String accumulatedString = "";  // Initialize an empty string to accumulate characters
  String alarmString = "";        // Initialize an empty string to store the alarm times

  for (int i = 0; i < inputString.length(); i++) {
    char currentChar = inputString[i];

    if (currentChar == '[' || currentChar == '"' || currentChar == ',' || currentChar == ']') {
      // Skip characters like '[', '"', or ',' and ']'
      continue;
    } else if (currentChar == '"') {
      // Print the accumulated string when a closing '"' is found
      if (accumulatedString != "") {
        if (alarmString != "") {
          alarmString += ",";
        }
        alarmString += accumulatedString;
        accumulatedString = "";  // Reset the accumulated string
      }
    } else {
      // Accumulate characters into the accumulated string
      accumulatedString += currentChar;
    }
  }

  // Print the last accumulated string after the loop ends (if any)
  if (accumulatedString != "") {
    if (alarmString != "") {
      alarmString += ",";
    }
    alarmString += accumulatedString;
  }

  Serial.println(alarmString);  // Print the accumulated alarm times
}

void StartAlarm() {
  Serial.println("called....");
  printNodes();
}

void ringing_sequence() {
  if (isRinging == true) {
    unsigned long currentMillis = millis();  // Get the current time in milliseconds

    if (currentMillis - previousMillis >= interval) {
      previousMillis = currentMillis;  // Update the previous time

      // Toggle the buzzer state
      digitalWrite(buzzerPin, !digitalRead(buzzerPin));
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
      String timeValue = inputWithoutBrackets.substring(startPos + 1, endPos - 1); // Skip the square bracket, space, and quotation mark after the comma
      // Serial.println(timeValue);
      addNode(timeValue);
      startPos = endPos + 2; // Skip the comma and space after it
    } else {
      String timeValue = inputWithoutBrackets.substring(startPos + 1, inputWithoutBrackets.length() - 2); // Skip the square bracket, space, and quotation mark
      // Serial.println(timeValue);
      addNode(timeValue);
    }
  }
}