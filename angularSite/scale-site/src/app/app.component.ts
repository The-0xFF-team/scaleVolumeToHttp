import { Component,OnInit } from '@angular/core';
import { HttpApiService } from './services/http-api.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit{
  title = 'scale-site';
  successfullQuery = false;
  lastQuantity:number = 0;
  constructor(private httpApi:HttpApiService){}
  ngOnInit(){
    setInterval(() => {
      this.Load();
    }, 1000);
  }

  Load():void{
    this.httpApi.getScaleState().subscribe({next:(a)=>{
      this.successfullQuery = true;
      this.lastQuantity = a.consumption;
    }, 
    error:(a) =>{
      this.successfullQuery = false;
      this.lastQuantity = 0;
    }
  });
  }
}
