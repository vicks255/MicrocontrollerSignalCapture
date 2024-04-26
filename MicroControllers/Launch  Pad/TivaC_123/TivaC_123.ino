int analogPin0 = A0;
int analogPin7 = A7;
int analogRefPin = A3;

int voltage0 = 0;
int voltage7 = 0;
int refVoltage = 0;
  
int window;
int sampleTime;
int channels;
  
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
      Serial.println("TivaC_123");
    }
    

    if(command == "GetSingle")
    {
      voltage0 = analogRead(analogPin0) - analogRead(analogRefPin);
      voltage7 = analogRead(analogPin7)  - analogRead(analogRefPin);
      Serial.println(String(voltage0) + "," + String(voltage7));
    }
    
      
    if(command == "window")
    {
      Serial.println("send_window");
      window = Serial.parseInt();
        
      Serial.println("send_sampling_interval");
      sampleTime = Serial.parseInt();
        
      Serial.println("send_channels");
      channels = Serial.parseInt();

      Serial.println("beginning_sampling");
        
      windowBeginTime = millis();
      switch(channels)
      {
        case 1:
          voltage0 = analogRead(analogPin0);
          voltageList = String(micros()) + "," + String(voltage0) + ",0,0";
          break;

        case 2:
          voltage7 = analogRead(analogPin7);
          voltageList = "0,0," + String(micros()) + "," + String(voltage7);
          break;

        case 12:
          voltage0 = analogRead(analogPin0);
          voltageList = String(micros()) + "," + String(voltage0);

          voltage7 = analogRead(analogPin7);
          voltageList = voltageList + "," + String(micros()) + "," + String(voltage7);
          break;
      }
        
      while((millis() - windowBeginTime) < window)
      {
        delayMicroseconds(sampleTime);
        switch(channels)
        {
          case 1:
            voltage0 = analogRead(analogPin0);
            voltageList = voltageList + "\r" + String(micros()) + "," + String(voltage0) + ",0,0";
            break;

          case 2:
            voltage7 = analogRead(analogPin7);
            voltageList = voltageList + "\r0,0,"  + String(micros()) + "," + String(voltage7);
            break;

          case 12:
            voltage0 = analogRead(analogPin0);
            voltageList = voltageList + "\r" + String(micros()) + "," + String(voltage0);

            voltage7 = analogRead(analogPin7);
            voltageList = voltageList + "," + String(micros()) + "," + String(voltage7);
            break;
        }
      }
      Serial.println(voltageList);
    }
  }
}
