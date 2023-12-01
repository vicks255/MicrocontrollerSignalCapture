  int analogPin = A0;
  int voltage = 0;
  
  int window;
  int sampleTime;
  
  long windowBeginTime;
  
  String command;
  String voltageList;


  void setup()
  {
    Serial.begin(9600);
  }


  void loop()
  {
    while(Serial.available() > 0)
    {
      command = Serial.readString();
      command.trim();

      if(command == "Model")
      {
        Serial.println("PortentaH7");
      }
      
      
      if(command == "window")
      {
        Serial.println("send_window");
        window = Serial.parseInt();
        
        Serial.println("send_sampling_interval");
        sampleTime = Serial.parseInt();
        
        Serial.println(command + " - " + String(window) + "ms - " + String(sampleTime) + "us");
        
        windowBeginTime = millis();
        voltage = analogRead(analogPin);
        voltageList = String(micros()) + "," + String(voltage);
        
        while((millis() - windowBeginTime) < window)
        {
          delayMicroseconds(sampleTime);
          voltage = analogRead(analogPin);
          voltageList = voltageList + "\r" + String(micros()) + "," + String(voltage);
        }
        Serial.println(voltageList);
      }


      if(command == "A0")
      {
        voltage = analogRead(analogPin);
        Serial.println(voltage);
      }
    }
  }
