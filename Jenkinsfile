pipeline {
  agent { label 'CerealAPI' }
  stages {
    stage('Build and test') {
      steps {
        step([$class: 'DockerComposeBuilder', dockerComposeFile: 'docker-compose.yml', option: [$class: 'StartAllServices'], useCustomDockerComposeFile: true])
      }
    } 
  }
}

