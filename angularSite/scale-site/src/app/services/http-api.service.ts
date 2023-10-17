import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { scaleState } from '../data.model';

@Injectable({
  providedIn: 'root'
})
export class HttpApiService {
  apiPath: string = "https://localhost:7186";
  constructor(private httpClient: HttpClient) { }

  getScaleState():Observable<scaleState>{
    return this.httpClient.get<scaleState>(this.apiPath);
  }
}
