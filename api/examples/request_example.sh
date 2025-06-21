curl -X POST "http://localhost:8000/api/v1/documentation" \
  -F "application_name=test-app" \
  -F "changed_sources=test.py" \
  -F "sources=@sources.zip" \
  -H "Content-Type: multipart/form-data"
