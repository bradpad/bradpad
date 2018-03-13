#include <Keyboard24.h>
#include <HID.h>

int prevState5 = 0;
int prevState6 = 0;
int prevState7 = 0;

void setup() {
  Serial.begin(9600);
  Keyboard.begin();
  pinMode(5, INPUT);
  pinMode(6, INPUT);
  pinMode(7, INPUT);
}

void loop() {
  if(digitalRead(5) == HIGH){
    if(prevState5 == LOW){
      Serial.println("press right button");
      Keyboard.press(KEY_F22);
      delay(10);
    }

    else{
      Serial.println("holding right button");
      Keyboard.press(KEY_F22);
      delay(10);
    }
  }
  else{
    Keyboard.release(KEY_F22);
  }
  
  if(digitalRead(6) == HIGH){
    if(prevState6 == LOW){
      Serial.println("press right button");
      Keyboard.press(KEY_F23);
      delay(10);
    }

    else{
      Serial.println("holding right button");
      Keyboard.press(KEY_F23);
      delay(10);
    }
  }
  else{
    Keyboard.release(KEY_F23);
  }
  
  if(digitalRead(7) == HIGH){
    if(prevState7 == LOW){
      Serial.println("press right button");
      Keyboard.press(KEY_F24);
      delay(10);
    }

    else{
      Serial.println("holding right button");
      Keyboard.press(KEY_F24);
      delay(10);
    }
  }
  else{
    Keyboard.release(KEY_F24);
  }
  
  prevState5 = digitalRead(5);
  prevState6 = digitalRead(6);
  prevState7 = digitalRead(7);
  
  delay(10);
  
}
