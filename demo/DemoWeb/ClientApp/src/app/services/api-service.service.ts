import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { catchError, delay, finalize, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ApiServiceService {
  private requestCounter$ = new BehaviorSubject<number>(0);

  pending$ = this.requestCounter$.pipe(
    map(counter => counter > 0),
    delay(0)
  );

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {}

  post<T>(url: string, params?: { [param: string]: any }, body?: any): Observable<T> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    this.updateCounter(true);

    return this.http.post<T>(this.baseUrl + url, JSON.stringify(body || {}), { headers, params }).pipe(
      catchError(error => {
        throw new Error(error);
      }),
      finalize(() => this.updateCounter(false))
    );
  }

  private updateCounter(increase: boolean) {
    this.requestCounter$.next(this.requestCounter$.value + (increase ? 1 : -1));
  }
}
