#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <ESP8266WebServer.h>
#include <EEPROM.h>
#include <SoftwareSerial.h>
#include <PubSubClient.h>

#define TIME 60
#define RETRY_AFTER 5

const char* mqtt_server = "test.mosquitto.org";
WiFiClient espClient;
PubSubClient client(espClient);
unsigned long lastMsg = 0;
#define MSG_BUFFER_SIZE (255)
char msg[MSG_BUFFER_SIZE];
int value = 0;

char buffer[1024];     // Buffer to store incoming payload
int bufferLength = 0;  // Length of data in the buffer
String messageString = "";

#define lamp D3
const char* lightState = "OFF";

//Variables
int i = 0;
int statusCode;
const char* ssid = "text";
const char* passphrase = "text";
String st;
String html;

SoftwareSerial mySerial(D7, D8);  // RX, TX


//Function Decalration
bool testWifi(void);
void launchWeb(void);
void setupAP(void);

//Establishing Local server at port 80 whenever required
ESP8266WebServer server(80);

void setup() {

  Serial.begin(115200);  //Initialising if(DEBUG)Serial Monitor
  mySerial.begin(9600);

  Serial.println();
  Serial.println("Disconnecting previously connected WiFi");
  WiFi.disconnect();
  EEPROM.begin(512);  //Initialasing EEPROM
  delay(10);
  pinMode(LED_BUILTIN, OUTPUT);

  digitalWrite(LED_BUILTIN, HIGH);

  Serial.println();
  Serial.println();
  Serial.println("Startup");

  setup_password();
}

void setup_password() {
  //---------------------------------------- Read eeprom for ssid and pass
  Serial.println("Reading EEPROM ssid");

  String esid;
  for (int i = 0; i < 32; ++i) {
    esid += char(EEPROM.read(i));
  }
  Serial.println();
  Serial.print("SSID: ");
  Serial.println(esid);
  Serial.println("Reading EEPROM pass");

  String epass = "";
  for (int i = 32; i < 96; ++i) {
    epass += char(EEPROM.read(i));
  }
  Serial.print("PASS: ");
  Serial.println(epass);


  WiFi.begin(esid.c_str(), epass.c_str());
  if (testWifi()) {
    Serial.println("Succesfully Connected!!!");

    client.setServer(mqtt_server, 1883);
    client.setCallback(callback);

    String response = make_http_request("https://iotalarmapp.onrender.com/get_alarm");
    mySerial.println(response);
    Serial.println("Response: " + response);
    return;
  } else {
    Serial.println("Turning the HotSpot On");
    launchWeb();
    setupAP();  // Setup HotSpot
  }

  Serial.println();
  Serial.print("Waiting");

  digitalWrite(LED_BUILTIN, HIGH);
  int timer_counter = 0;
  while ((WiFi.status() != WL_CONNECTED)) {
    Serial.print(".");
    if ((timer_counter % 5000) == 0) {
      Serial.println();
      digitalWrite(LED_BUILTIN, LOW);
    }
    server.handleClient();
    if (timer_counter >= (RETRY_AFTER * 60 * 1000)) {
      break;
    }
    delay(100);
    digitalWrite(LED_BUILTIN, HIGH);
    timer_counter += 100;
  }
  setup_password();
}


void loop() {
  if ((WiFi.status() == WL_CONNECTED)) {
    main_program();
  } else {
    // Try to reconnect
    setup_password();
  }
}


//----------------------------------------------- Fuctions used for WiFi credentials saving and connecting to it which you do not need to change
bool testWifi(void) {
  digitalWrite(LED_BUILTIN, HIGH);
  int c = 0;
  Serial.println("Waiting for Wifi to connect");
  while (c < (2 * TIME)) {
    if (WiFi.status() == WL_CONNECTED) {
      return true;
    }
    delay(500);
    Serial.print("*");
    c++;
  }
  Serial.println("");
  Serial.println("Connect timed out, opening AP");
  return false;
}

void launchWeb() {
  Serial.println("");
  if (WiFi.status() == WL_CONNECTED)
    Serial.println("WiFi connected");
  Serial.print("Local IP: ");
  Serial.println(WiFi.localIP());
  Serial.print("SoftAP IP: ");
  Serial.println(WiFi.softAPIP());
  server.on("/", handleRoot);  // Add this line to handle the root page
  server.on("/data", handleData);
  // Start the server
  server.begin();
  Serial.println("Server started");
}

void setupAP(void) {
  WiFi.mode(WIFI_STA);
  WiFi.disconnect();
  delay(100);
  int n = WiFi.scanNetworks();
  Serial.println("scan done");
  if (n == 0)
    Serial.println("no networks found");
  else {
    Serial.print(n);
    Serial.println(" networks found");
    for (int i = 0; i < n; ++i) {
      // Print SSID and RSSI for each network found
      Serial.print(i + 1);
      Serial.print(": ");
      Serial.print(WiFi.SSID(i));
      Serial.print(" (");
      Serial.print(WiFi.RSSI(i));
      Serial.print(")");
      Serial.println((WiFi.encryptionType(i) == ENC_TYPE_NONE) ? " " : "*");
      delay(10);
    }
  }
  Serial.println("");
  st = "<ol>";
  for (int i = 0; i < n; ++i) {
    // Print SSID and RSSI for each network found
    st += "<li>";
    st += WiFi.SSID(i);
    st += " (";
    st += WiFi.RSSI(i);

    st += ")";
    st += (WiFi.encryptionType(i) == ENC_TYPE_NONE) ? " " : "*";
    st += "</li>";
  }
  st += "</ol>";
  delay(100);
  WiFi.softAP("NodeMCU", "tiktok14");
  Serial.println("softap");
  launchWeb();
  Serial.println("over");
}

void handleRoot() {
  html = "<html>";
  html += "<head>";
  html += "<style>";
  html += "body {";
  html += "  font-family: Arial, sans-serif;";
  html += "  background-color: #f7f7f7;";
  html += "}";
  html += "form {";
  html += "  max-width: 400px;";
  html += "  margin: 0 auto;";
  html += "  background-color: #fff;";
  html += "  border-radius: 5px;";
  html += "  padding: 20px;";
  html += "  box-shadow: 0px 0px 10px 0px rgba(0,0,0,0.5);";
  html += "}";
  html += "input[type=text], input[type=password] {";
  html += "  width: 100%;";
  html += "  padding: 12px 20px;";
  html += "  margin: 8px 0;";
  html += "  box-sizing: border-box;";
  html += "  border: solid black 1px;";
  html += "  border-radius: 4px;";
  html += "}";
  html += "button {";
  html += "  background-color: #4CAF50;";
  html += "  color: white;";
  html += "  padding: 14px 20px;";
  html += "  margin: 8px 0;";
  html += "  border: none;";
  html += "  border-radius: 4px;";
  html += "  cursor: pointer;";
  html += "}";
  html += "button:hover {";
  html += "  background-color: #45a049;";
  html += "}";
  html += "</style>";
  html += "</head>";
  html += "<body>";
  html += "<form>";
  html += "<label for='name'>SSID:</label>";
  html += "<input type='text' id='name' name='name'>";
  html += "<label for='password'>PASSWORD:</label>";
  html += "<input type='password' id='password' name='password'>";
  html += "<button type='button' onclick='submitData()'>Submit</button>";
  html += "</form>";
  html += "<div id='feedback-div' style='display: flex; justify-content: center; align-items: center;'>";
  html += "  <p id='feedback-label' style='text-align: center;'>This is the feedback</p>";
  html += "</div>";

  html += "<script>";
  html += "function submitData() {";
  html += "var name = document.getElementById('name').value;";
  html += "var password = document.getElementById('password').value;";
  html += "var feedback_label = document.getElementById('feedback-label');";
  html += "var xhttp = new XMLHttpRequest();";
  html += "xhttp.onreadystatechange = function() {";
  html += "  if (this.readyState == 4 && this.status == 200) {";
  html += "    feedback_label.innerText = this.responseText;";
  html += "    console.log(this.responseText);";
  html += "  }";
  html += "};";
  html += "xhttp.open('POST', '/data', true);";
  html += "xhttp.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');";
  html += "xhttp.send('name=' + name + '&password=' + password);";
  html += "}";
  html += "</script>";
  html += "</body>";
  html += "</html>";
  server.send(200, "text/html", html);
}

void handleData() {
  if (server.method() == HTTP_POST) {
    String name = server.arg("name");
    String password = server.arg("password");
    Serial.print("SSID: ");
    Serial.println(name);
    Serial.print("Password: ");
    Serial.println(password);

    String qsid = name;
    String qpass = password;

    blink_led(100);

    if (qsid.length() > 63 || qpass.length() > 63) {
      Serial.println("Error Occurred");
      server.send(200, "text/html", "Long SSID or password. Please Retry");
      return;
    }

    if (qsid.length() <= 0 || qpass.length() <= 0) {
      server.send(200, "text/html", "All fields required");
      return;
    }

    if (qsid.length() > 0 && qpass.length() > 0) {
      server.send(200, "text/html", "Successfully saved credentials. Connecting to the new Wi-Fi...");
      Serial.println("Successfully saved credentials. Connecting to the new Wi-Fi...");

      Serial.println("clearing eeprom");
      for (int i = 0; i < 96; ++i) {
        EEPROM.write(i, 0);
      }
      Serial.println(qsid);
      Serial.println("");
      Serial.println(qpass);
      Serial.println("");

      Serial.println("writing eeprom ssid:");
      for (int i = 0; i < qsid.length(); ++i) {
        EEPROM.write(i, qsid[i]);
        Serial.print("Wrote: ");
        Serial.println(qsid[i]);
      }
      Serial.println("writing eeprom pass:");
      for (int i = 0; i < qpass.length(); ++i) {
        EEPROM.write(32 + i, qpass[i]);
        Serial.print("Wrote: ");
        Serial.println(qpass[i]);
      }
      EEPROM.commit();

      ESP.reset();
    } else {
      server.send(200, "text/html", "An error occured. Please retry");
      Serial.println("Sending 404");
    }
    server.sendHeader("Access-Control-Allow-Origin", "*");
  }
}

void main_program() {
  start_mqtt();
}


void blink_led(int the_delay) {
  digitalWrite(LED_BUILTIN, LOW);
  delay(the_delay);
  digitalWrite(LED_BUILTIN, HIGH);
}

void start_mqtt() {
  if (!client.connected()) {
    reconnect();
  }
  client.loop();

  unsigned long now = millis();
  if (now - lastMsg > 2000) {
    lastMsg = now;
    ++value;
    snprintf(msg, MSG_BUFFER_SIZE, lightState, value);
    Serial.print("Led status: ");
    Serial.println(msg);
    client.publish("esp/led/status", msg);
  }
}


void callback(char* topic, byte* payload, unsigned int length) {
  if (strcmp(topic, "esp/alarm") == 0) {
    for (int i = 0; i < length; i++) {
      if (payload[i] == '\n') {
        get_message(topic, messageString);
        messageString = "";
      } else {
        messageString += (char)payload[i];
      }
    }
  } else if (strcmp(topic, "esp/led") == 0) {
    String my_message = "";
    for (int i = 0; i < length; i++) {
      my_message += (char)payload[i];
    }
    get_message(topic, my_message);
  }
}

void get_message(char* topic, String messageString) {
  if (strcmp(topic, "esp/led") == 0) {
    // Switch on the LED if an 1 was received as first character
    if (messageString == "1") {
      digitalWrite(BUILTIN_LED, LOW);  // Turn the LED on (Note that LOW is the voltage level
      digitalWrite(lamp, HIGH);
      lightState = "ON";
      // but actually the LED is on; this is because
      // it is active low on the ESP-01)
    } else if (messageString == "0") {
      digitalWrite(BUILTIN_LED, HIGH);  // Turn the LED off by making the voltage HIGH
      digitalWrite(lamp, LOW);
      lightState = "OFF";
    }
  } else if (strcmp(topic, "esp/alarm") == 0) {
    Serial.print("Message arrived [");
    Serial.print(topic);
    Serial.print("] ");
    String extractedData = extractDataFromString(messageString);
    mySerial.println(extractedData);
    Serial.println(extractedData);
  }
}

String extractDataFromString(String data) {
  int start = data.indexOf('[');  // Find the index of '['
  int end = data.indexOf(']');    // Find the index of ']'
  if (start != -1 && end != -1 && end > start) {
    // Extract the data, including the square brackets
    String extractedData = data.substring(start, end + 1);
    return extractedData;
  } else {
    return "";  // Return empty string if no data is found
  }
}

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);
    // Attempt to connect
    if (client.connect(clientId.c_str())) {
      Serial.println("connected");
      // Once connected, publish an announcement...
      client.publish("esp/led/status", lightState);
      // ... and resubscribe
      client.subscribe("esp/led");

      client.subscribe("esp/alarm");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}


String make_http_request(String url) {
  String response = "";
  
  WiFiClientSecure client; // Use WiFiClientSecure for HTTPS
  client.setInsecure(); // Allow insecure connections for testing purposes only. Use client.setCACert() with a CA certificate for secure connections in production.
  
  HTTPClient http;
  
  http.begin(client, url); // Specify the client and URL
  http.setAuthorization("admin", "admin"); // Set basic authentication credentials
  int httpCode = http.GET(); // Send HTTP GET request
  
  if (httpCode > 0) { // Check for a valid HTTP response
    if (httpCode == HTTP_CODE_OK) { // Check for a successful response
      response = http.getString(); // Get the response body as a String
    } else {
      Serial.printf("HTTP request failed with error code: %d\n", httpCode);
    }
  } else {
    Serial.println("Failed to connect to server");
  }
  
  http.end(); // Close the HTTP connection
  return response;
}