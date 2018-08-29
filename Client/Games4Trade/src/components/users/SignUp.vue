<template>
<div>
    <div id="signup-form" class="row">
        <div class="form rounded col-6 offset-3 p-4">
            <form >
                <div class="form-group">
                    <label for="email">Adres e-mail</label>
                    <input
                            type="text"
                            id="email"
                            class="form-control"
                            @blur="$v.email.$touch()"
                            v-model="email">
                </div>
                <div class="form-group">
                    <label for="login">Login</label>
                    <input
                            type="text"
                            id="login"
                            class="form-control"
                            @blur="$v.login.$touch()"
                            v-model.lazy="login">
                </div>
                <div class="form-group">
                    <label for="password">Hasło</label>
                    <input
                            type="password"
                            id="password"
                            class="form-control"
                            @blur="$v.password.$touch()"
                            v-model="password">
                </div>
                <div class="form-group">
                    <label for="confirmPassword">Powtórz hasło</label>
                    <input
                            type="password"
                            id="confirmPassword"
                            class="form-control"
                            @blur="$v.confirmPassword.$touch()"
                            v-model="confirmPassword">
                </div>
                <div class="submit">
                    <button type="submit" class="btn btn-info btn-block" :disabled="$v.$invalid">Utwórz konto !</button>
                </div>
            </form>
            <p>{{ $v }}</p>
        </div>
    </div>
</div>
</template>

<script>
import { required, email, sameAs, minLength } from 'vuelidate/lib/validators'
import axios from 'axios'
export default {
  name: 'signup',
  data () {
    return {
      login: '',
      password: '',
      confirmPassword: '',
      email: ''
    }
  },
  validations: {
    login: {
      required,
      unique: val => {
        if (val === '') return true
        return axios.head(`login?login=${val}`)
          .then(() => { return false })
          .catch(error => {
            console.log(error.response.status)
            return error.response.status === 404
          })
      }
    },
    email: {
      required,
      email
    },
    password: {
      required,
      minLen: minLength(6)
    },
    confirmPassword: {
      sameAs: sameAs('password')
    }
  }
}
</script>

<style scoped>
    .form{
        background-color: whitesmoke;
        border: solid 1px #26bba6;
    }
</style>
