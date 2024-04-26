int analogPin0 = A0;
int analogPin6 = A6;

int voltage0 = 0;
int voltage6 = 0;
  
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
      Serial.println("PortentaH7");
    }

    if(command == "GetSingle")
    {
      int loopCount;
      voltage0 = 0;
      voltage6 = 0;
      for(loopCount = 0; loopCount < 20; loopCount++)
      {
        voltage0 += analogRead(A0);
        voltage6 += analogRead(A6);
      }

      voltage0 = voltage0 / 20;
      voltage6 = voltage6 / 20;
      Serial.println(String(voltage0) + "," + String(voltage6));
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
          voltage6 = analogRead(analogPin6);
          voltageList = "0,0," + String(micros()) + "," + String(voltage6);
          break;

        case 12:
          voltage0 = analogRead(analogPin0);
          voltageList = String(micros()) + "," + String(voltage0);

          voltage6 = analogRead(analogPin6);
          voltageList = voltageList + "," + String(micros()) + "," + String(voltage6);
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
            voltage6 = analogRead(analogPin6);
            voltageList = voltageList + "\r0,0,"  + String(micros()) + "," + String(voltage6);
            break;

          case 12:
            voltage0 = analogRead(analogPin0);
            voltageList = voltageList + "\r" + String(micros()) + "," + String(voltage0);

            voltage6 = analogRead(analogPin6);
            voltageList = voltageList + "," + String(micros()) + "," + String(voltage6);
            break;
        }
      }
      Serial.println(voltageList);
    }
  }
}
