# ab

## 内网穿透

### MockSocket

```
  ⚡wdora ❯❯ ab -n 1000 -c 10 http://wdora.com:8080/
This is ApacheBench, Version 2.0.40-dev <$Revision: 1.146 $> apache-2.0
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Copyright 1997-2005 The Apache Software Foundation, http://www.apache.org/

Benchmarking wdora.com (be patient)
Completed 100 requests
Completed 200 requests
Completed 300 requests
Completed 400 requests
Completed 500 requests
Completed 600 requests
Completed 700 requests
Completed 800 requests
Completed 900 requests
Finished 1000 requests


Server Software:        Microsoft-IIS/10.0
Server Hostname:        wdora.com
Server Port:            8080

Document Path:          /
Document Length:        696 bytes

Concurrency Level:      10
Time taken for tests:   11.115058 seconds
Complete requests:      1000
Failed requests:        0
Write errors:           0
Total transferred:      963000 bytes
HTML transferred:       696000 bytes
Requests per second:    89.97 [#/sec] (mean)
Time per request:       111.151 [ms] (mean)
Time per request:       11.115 [ms] (mean, across all concurrent requests)
Transfer rate:          84.57 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        2    6  45.1      4    1017
Processing:    35  103 176.2     61    1163
Waiting:       34  103 174.3     60    1163
Total:         40  110 181.2     65    1168

Percentage of the requests served within a certain time (ms)
  50%     65
  66%     67
  75%     68
  80%     69
  90%    110
  95%    320
  98%   1065
  99%   1087
 100%   1168 (longest request)
```

### frp

```
  ⚡wdora ❯❯ ab -n 1000 -c 10 http://wdora.com:8079/
This is ApacheBench, Version 2.0.40-dev <$Revision: 1.146 $> apache-2.0
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Copyright 1997-2005 The Apache Software Foundation, http://www.apache.org/

Benchmarking wdora.com (be patient)
Completed 100 requests
Completed 200 requests
Completed 300 requests
Completed 400 requests
Completed 500 requests
Completed 600 requests
Completed 700 requests
Completed 800 requests
Completed 900 requests
Finished 1000 requests


Server Software:        Microsoft-IIS/10.0
Server Hostname:        wdora.com
Server Port:            8079

Document Path:          /
Document Length:        696 bytes

Concurrency Level:      10
Time taken for tests:   12.990606 seconds
Complete requests:      1000
Failed requests:        0
Write errors:           0
Total transferred:      963000 bytes
HTML transferred:       696000 bytes
Requests per second:    76.98 [#/sec] (mean)
Time per request:       129.906 [ms] (mean)
Time per request:       12.991 [ms] (mean, across all concurrent requests)
Transfer rate:          72.36 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        2   10  77.8      4    1015
Processing:    10  118 267.2     42    2398
Waiting:       10  101 256.9     28    2397
Total:         12  129 276.6     46    2401

Percentage of the requests served within a certain time (ms)
  50%     46
  66%     47
  75%     48
  80%     49
  90%    251
  95%   1049
  98%   1054
  99%   1279
 100%   2401 (longest request)
```

## 端口转发

### MockSocket

```
  ⚡wdora ❯❯ ab -n 5000 -c 10 http://localhost:8080/iisstart.png
This is ApacheBench, Version 2.0.40-dev <$Revision: 1.146 $> apache-2.0
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Copyright 1997-2005 The Apache Software Foundation, http://www.apache.org/

Benchmarking localhost (be patient)
Completed 500 requests
Completed 1000 requests
Completed 1500 requests
Completed 2000 requests
Completed 2500 requests
Completed 3000 requests
Completed 3500 requests
Completed 4000 requests
Completed 4500 requests
Finished 5000 requests


Server Software:        Microsoft-IIS/10.0
Server Hostname:        localhost
Server Port:            8080

Document Path:          /iisstart.png
Document Length:        98757 bytes

Concurrency Level:      10
Time taken for tests:   1.330081 seconds
Complete requests:      5000
Failed requests:        0
Write errors:           0
Total transferred:      495130000 bytes
HTML transferred:       493785000 bytes
Requests per second:    3759.17 [#/sec] (mean)
Time per request:       2.660 [ms] (mean)
Time per request:       0.266 [ms] (mean, across all concurrent requests)
Transfer rate:          363530.49 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.3      0       1
Processing:     0    2   0.7      2       8
Waiting:        0    0   0.7      0       6
Total:          0    2   0.7      2       8

Percentage of the requests served within a certain time (ms)
  50%      2
  66%      3
  75%      3
  80%      3
  90%      3
  95%      3
  98%      3
  99%      3
 100%      8 (longest request)
```

### Nginx(tcp)

```
  ⚡wdora ❯❯ ab -n 5000 -c 10 http://localhost:8082/iisstart.png
This is ApacheBench, Version 2.0.40-dev <$Revision: 1.146 $> apache-2.0
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Copyright 1997-2005 The Apache Software Foundation, http://www.apache.org/

Benchmarking localhost (be patient)
Completed 500 requests
Completed 1000 requests
Completed 1500 requests
Completed 2000 requests
Completed 2500 requests
Completed 3000 requests
Completed 3500 requests
Completed 4000 requests
Completed 4500 requests
Finished 5000 requests


Server Software:        Microsoft-IIS/10.0
Server Hostname:        localhost
Server Port:            8082

Document Path:          /iisstart.png
Document Length:        98757 bytes

Concurrency Level:      10
Time taken for tests:   1.737964 seconds
Complete requests:      5000
Failed requests:        0
Write errors:           0
Total transferred:      495130000 bytes
HTML transferred:       493785000 bytes
Requests per second:    2876.93 [#/sec] (mean)
Time per request:       3.476 [ms] (mean)
Time per request:       0.348 [ms] (mean, across all concurrent requests)
Transfer rate:          278213.48 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.2      0       1
Processing:     1    3   0.6      3       4
Waiting:        0    2   0.7      2       4
Total:          1    3   0.6      3       5

Percentage of the requests served within a certain time (ms)
  50%      3
  66%      3
  75%      3
  80%      4
  90%      4
  95%      4
  98%      4
  99%      4
 100%      5 (longest request)
```

### Nginx(http)

```
  ⚡wdora ❯❯ ab -n 5000 -c 10 http://localhost:8081/iisstart.png
This is ApacheBench, Version 2.0.40-dev <$Revision: 1.146 $> apache-2.0
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Copyright 1997-2005 The Apache Software Foundation, http://www.apache.org/

Benchmarking localhost (be patient)
Completed 500 requests
Completed 1000 requests
Completed 1500 requests
Completed 2000 requests
Completed 2500 requests
Completed 3000 requests
Completed 3500 requests
Completed 4000 requests
Completed 4500 requests
Finished 5000 requests


Server Software:        nginx/1.22.0
Server Hostname:        localhost
Server Port:            8081

Document Path:          /iisstart.png
Document Length:        98757 bytes

Concurrency Level:      10
Time taken for tests:   2.219142 seconds
Complete requests:      5000
Failed requests:        0
Write errors:           0
Total transferred:      495100000 bytes
HTML transferred:       493785000 bytes
Requests per second:    2253.12 [#/sec] (mean)
Time per request:       4.438 [ms] (mean)
Time per request:       0.444 [ms] (mean, across all concurrent requests)
Transfer rate:          217875.20 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.3      0       1
Processing:     1    3   1.2      4       8
Waiting:        1    3   0.7      3       7
Total:          1    4   0.6      4       8

Percentage of the requests served within a certain time (ms)
  50%      4
  66%      4
  75%      4
  80%      5
  90%      5
  95%      5
  98%      5
  99%      5
 100%      8 (longest request)
```