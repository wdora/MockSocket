# ab

## 2022年9月22日

  ⚡wdora ❯❯ ab -n 5000 -c 10 http://localhost:8080/
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

Document Path:          /
Document Length:        696 bytes

Concurrency Level:      10
Time taken for tests:   4.51029 seconds
Complete requests:      5000
Failed requests:        0
Write errors:           0
Total transferred:      4815000 bytes
HTML transferred:       3480000 bytes
Requests per second:    1234.25 [#/sec] (mean)
Time per request:       8.102 [ms] (mean)
Time per request:       0.810 [ms] (mean, across all concurrent requests)
Transfer rate:          1160.69 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.5      0       2
Processing:     1    7   2.8      7      19
Waiting:        1    6   2.7      6      17
Total:          1    7   2.9      7      19

Percentage of the requests served within a certain time (ms)
  50%      7
  66%      9
  75%      9
  80%     10
  90%     12
  95%     12
  98%     13
  99%     13
 100%     19 (longest request)