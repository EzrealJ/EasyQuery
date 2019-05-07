# EasyQuery
## 从你的请求构造一个查询表达式。

* 用于AspNetCore、依赖AspNetCore且暂时没有支持其它Web框架的计划,如果需要自己拿去改
* 不是高端的东西，看代码就知道很简单，只是从请求构造一个查询/排序表达式
* 复杂需求建议移步GraphQL
* 不接受杠精指教
* 欢迎建议

## v0.1.3.1
* 修复了InvokeCriteriaFilter的bug，该bug曾导致过滤器不能正常执行
* 增加了demo
* 不建议在Query参数中使用,即使你在语义上是get请求，也建议使用post请求来传递条件